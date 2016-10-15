using System;

namespace BetManager.Core.Processors
{
    public interface IProcessor : IDisposable
    { 
        string Url { get; }

        string Process();
    }
}