using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Messenger
{
    internal class MongoMessengerRepository : IMessengerRepository
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoMessengerRepository> _log;

        private IMongoCollection<AccessDocument>? _accessCollection;
        private IMongoCollection<InviteDocument>? _inviteCollection;

        private List<AccessDocument> _access = default!;

        public MongoMessengerRepository(
            IMongoDatabase database,
            ILogger<MongoMessengerRepository> log)
        {
            _database = database;
            _log = log;
            _inviteCollection = _database
                .GetCollection<InviteDocument>("Invites");
        }

        public async Task StartAsync(MessengerType type)
        {
            _accessCollection = _database
                .GetCollection<AccessDocument>(type.ToString() + "Access");

            // cannot it be filled from the outside?
            _access = await _accessCollection
                .Find(FilterDefinition<AccessDocument>.Empty)
                .ToListAsync();
        }

        public bool TryGetCurrentChannel(string chatId, out AccessRecord channel)
        {
            var record = _access.FirstOrDefault(
                x => x.ChatId == chatId && x.IsCurrent);

            if (record != null)
            {
                channel = new AccessRecord
                {
                    ChannelId = record.ChannelId,
                    ChatId = record.ChatId,
                    IsCurrent = record.IsCurrent,
                    ProjectId = record.ProjectId,
                    ProjectName = record.ProjectName
                };
            }
            else
            {
                channel = default!;
            }

            return channel != default!;
        }

        public bool TryGetChat(Guid channelId, out string chatId)
        {
            var record = _access.FirstOrDefault(
                x => x.ChannelId == channelId);

            chatId = record != null ? record.ChatId : String.Empty;

            return chatId != null;
        }

        public bool TryGetChannels(string chatId, out AccessRecord[] channels)
        {
            channels = _access
                .Where(x => x.ChatId == chatId)
                .Select(x => new AccessRecord
                {
                    ChatId = x.ChatId,
                    IsCurrent = x.IsCurrent,
                    ChannelId = x.ChannelId,
                    ProjectName = x.ProjectName,
                    ProjectId = x.ProjectId
                })
                .ToArray();

            return channels.Any();
        }

        public async Task SetCurrentProject(string chatId, string projectName)
        {
            var newAccess = _access.FirstOrDefault(
                x => x.ChatId == chatId && x.ProjectName == projectName);

            if (newAccess == default!)
            {
                throw new ArgumentException("ChatId or ProjectName not found");
            }

            // disable old seleceted access
            var oldCurrentAccess = _access.FirstOrDefault(
                x => x.ChatId == chatId && x.IsCurrent);

            if (oldCurrentAccess != default)
            {
                oldCurrentAccess.IsCurrent = false;

                await _accessCollection.ReplaceOneAsync(
                    x => x.Id == oldCurrentAccess.Id,
                    oldCurrentAccess);
            }

            // enable new access
            newAccess.IsCurrent = true;

            await _accessCollection.ReplaceOneAsync(
                x => x.Id == newAccess.Id,
                newAccess);
        }

        public async Task JoinToProject(
            string chatId,
            Guid channelId,
            string projectName,
            Guid projectId)
        {
            var access = new AccessDocument
            {
                Id = Guid.NewGuid(),
                ChatId = chatId,
                ChannelId = channelId,
                ProjectName = projectName,
                ProjectId = projectId,
                IsCurrent = true
            };

            await _accessCollection!.InsertOneAsync(access);

            _access.Add(access);

            await SetCurrentProject(chatId, projectName);
        }

        public async Task LeaveProject(
            string chatId,
            Guid channelId,
            string projectName)
        {
            var currentProject = _access
                .Where(x=>x.IsCurrent)
                .FirstOrDefault();

            if (currentProject == null)
            {
                throw new Exception("Current project not found");
            }

            if (currentProject.ProjectName != projectName)
            {
                throw new Exception("Bad current project name");
            }

            currentProject.IsCurrent = false;
            await _accessCollection.ReplaceOneAsync(
                x => x.Id == currentProject.Id,
                currentProject);
        }

        public async Task CreateInvite(string code, Guid projectId = default!)
        {
            var invite = new InviteDocument
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                Code = code,
                ProjectId = projectId
            };

            await _inviteCollection!.InsertOneAsync(invite);
        }

        // TODO: check if it is stale ?
        public bool CheckInvite(string code, out Guid projectId)
        {
            var invite = _inviteCollection
                .Find(x => x.Code == code)
                .FirstOrDefault();

            projectId = invite?.ProjectId ?? default;

            return invite != default;
        }

        public async Task DeleteInvite(string code)
        {
            await _inviteCollection.DeleteOneAsync(x => x.Code == code);
        }

        public async Task OnProjectDeleted(Guid projectId)
        {
            await _inviteCollection.DeleteManyAsync(x => x.ProjectId == projectId);
        }

        public async Task OnChannelDeleted(Guid channelId)
        {
            _access.RemoveAll(x => x.ChannelId == channelId);

            await _accessCollection.DeleteManyAsync(
                x => x.ChannelId == channelId);
        }
    }
}
