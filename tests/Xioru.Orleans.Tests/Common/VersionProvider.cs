using System;
using System.Reflection;
using Xioru.Grain.Contracts;

namespace Xioru.Orleans.Tests.Common
{
    public class VersionProvider : IVersionProvider
    {
        public Version GetVersion()
        {
            return Assembly
                .GetExecutingAssembly()
                .GetName()
                .Version!;
        }
    }
}
