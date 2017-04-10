using System;

namespace UGFramework.Pool
{
    /**
     * --- DOC BEGIN ---
     * --- DOC END ---
     */
    public class ObjectPool : SingleTon<ObjectPool>
    {
        public void PreAlloc<T>(uint count, params object[] args)
        {
        }

        public T Alloc<T>(params object[] args)
            where T : class, IObject
        {
            T obj = Activator.CreateInstance(typeof(T), args) as T;
            obj.Alloc();
            return obj;
        }

        public void Dealloc(IObject obj)
        {
            obj.Dealloc(); 
        }
    }
}