using System.Collections.Generic;

namespace UGFramework.Pool
{
    /**
     * --- DOC BEGIN ---
     * --- DOC END ---
     */
    public class UnusedObjsCache
    {
        Dictionary<string, Queue<IObject>> _objs = new Dictionary<string, Queue<IObject>>();

        void Ensure(string key)
        {
            if (_objs.ContainsKey(key) == false)
                _objs[key] = new Queue<IObject>();
        }

        public void Push(string key, IObject obj)
        {
            this.Ensure(key);
            _objs[key].Enqueue(obj);
        }

        public IObject Pop(string key)
        {
            this.Ensure(key);
            if (_objs[key].Count <= 0)
                return null;
            return _objs[key].Dequeue();
        }
    }
}