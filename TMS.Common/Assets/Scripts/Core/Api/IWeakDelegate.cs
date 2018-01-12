using System;

namespace TMS.Common.Core
{
    public interface IWeakDelegate : IDisposable
    {
        string Id { get; }
        
        bool IsAlive { get; }
        
        object Target { get; }

        void BeginInvoke(params object[] args);
        
        object Invoke(params object[] args);
    }
}