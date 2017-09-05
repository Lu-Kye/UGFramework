using System;
using UGFramework.Log;

namespace UGFramework.Pool
{
    /**
     * --- DOC BEGIN ---
     * --- DOC END ---
     */
    public class ObjectPool<T> : SingleTon<T>
        where T : ObjectPool<T>
    {
        bool _enabled = true;
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        } 

        protected UnusedObjsCache _unusedsCache = new UnusedObjsCache();
        protected UsingObjsCache _usingsCache = new UsingObjsCache();

        public void PreAlloc<TObj>(uint count, params object[] args)
        {
        }

        public virtual TObj Alloc<TObj>(object[] args, string key = null)
            where TObj : class, IObject
        {
            if (string.IsNullOrEmpty(key))
            {
                var type = typeof(TObj);
                key = type.FullName;
            }

            var obj = _unusedsCache.Pop(key) as TObj;
            if (obj == null)
                obj = Activator.CreateInstance(typeof(TObj)) as TObj;
            obj.Alloc(args);
            _usingsCache.PushKey(key, obj);
            return obj;
        }

        public virtual void Dealloc<TObj>(TObj obj)
            where TObj : class, IObject
        {
            var key = _usingsCache.PopKey(obj);
            if (string.IsNullOrEmpty(key))
            {
                LogManager.Error(string.Format(
                    "Dealloc error, uncached obj!"
                ), this);
                return;
            }
            obj.Dealloc(); 
            _unusedsCache.Push(key, obj);
        }
    }
}