using System;

namespace UGFramework.Pool
{
    /**
     * --- DOC BEGIN ---
     * --- DOC END ---
     */
    public class ObjectPool : SingleTon<ObjectPool>
    {
        public T Alloc<T>(params object[] args)
            where T : class, IObject
        {
            T obj = Activator.CreateInstance(typeof(T), args) as T;
            obj.Init();
            return obj;
        }

        public void Dealloc(IObject obj)
        {
            obj.Dispose(); 
        }
    }
}