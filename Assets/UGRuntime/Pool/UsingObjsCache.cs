using System.Collections.Generic;

namespace UGFramework.Pool
{
    /**
     * --- DOC BEGIN ---
     * --- DOC END ---
     */
    public class UsingObjsCache
    {
        Dictionary<IObject, string> _objKeys = new Dictionary<IObject, string>();

        public void PushKey(string key, IObject obj)
        {
            _objKeys[obj] = key;
        }

        public string PopKey(IObject obj)
        {
            string key = null;
            if (_objKeys.ContainsKey(obj))
            {
                key = _objKeys[obj];
                _objKeys.Remove(obj);
            }
            return key;
        }
    }
}