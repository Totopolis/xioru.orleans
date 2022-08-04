using AutoMapper;
using Xioru.Grain.Contracts.User;
using Xioru.Grain.User;
using Xioru.Messaging.Contracts.Channel;

namespace Xioru.Messaging.Channel
{
    public class ChannelMapper : Profile
    {
        public ChannelMapper()
        {
            CreateMap<ChannelState, ChannelProjection>();
            CreateMap<CreateChannelCommandModel, ChannelState>();
            CreateMap<UpdateChannelCommandModel, ChannelState>();
        }
    }
}
