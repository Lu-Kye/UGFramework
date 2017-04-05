using System;

namespace UGFramework.Pool
{
    public interface IObject : IDisposable
    {
        void Init();    
    }
}