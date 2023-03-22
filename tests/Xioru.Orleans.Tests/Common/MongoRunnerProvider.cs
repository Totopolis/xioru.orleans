using EphemeralMongo;
using System;

namespace Xioru.Orleans.Tests.Common;

public static class MongoRunnerProvider
{
    private static readonly object _lockObj = new object();
    private static IMongoRunner? _runner;
    private static int _useCounter;

    public static IMongoRunner Get()
    {
        lock (_lockObj)
        {
            if (_runner == null)
            {
                _runner = MongoRunner.Run(new MongoRunnerOptions
                {
                    /*UseSingleNodeReplicaSet = true,
                    ReplicaSetSetupTimeout = TimeSpan.FromSeconds(30),*/
                    AdditionalArguments = "--quiet",
                    KillMongoProcessesWhenCurrentProcessExits = true
                });
            }

            _useCounter++;
            return new MongoRunnerWrapper(_runner);
        }
    }

    public sealed class MongoRunnerWrapper : IMongoRunner
    {
        private IMongoRunner? _underlyingMongoRunner;

        public MongoRunnerWrapper(IMongoRunner underlyingMongoRunner)
        {
            this._underlyingMongoRunner = underlyingMongoRunner;
        }

        public string ConnectionString
        {
            get => this._underlyingMongoRunner?.ConnectionString ?? throw new ObjectDisposedException(nameof(IMongoRunner));
        }

        public void Import(string database, string collection, string inputFilePath, string? additionalArguments = null, bool drop = false)
        {
            if (this._underlyingMongoRunner == null)
            {
                throw new ObjectDisposedException(nameof(IMongoRunner));
            }

            this._underlyingMongoRunner.Import(database, collection, inputFilePath, additionalArguments, drop);
        }

        public void Export(string database, string collection, string outputFilePath, string? additionalArguments = null)
        {
            if (this._underlyingMongoRunner == null)
            {
                throw new ObjectDisposedException(nameof(IMongoRunner));
            }

            this._underlyingMongoRunner.Export(database, collection, outputFilePath, additionalArguments);
        }

        public void Dispose()
        {
            if (this._underlyingMongoRunner != null)
            {
                this._underlyingMongoRunner = null;
                StaticDispose();
            }
        }

        private static void StaticDispose()
        {
            lock (_lockObj)
            {
                if (_runner != null)
                {
                    _useCounter--;
                    if (_useCounter == 0)
                    {
                        _runner.Dispose();
                        _runner = null;
                    }
                }
            }
        }
    }
}
