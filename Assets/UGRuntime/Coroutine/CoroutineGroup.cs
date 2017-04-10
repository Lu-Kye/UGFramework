using System;
using UGFramework.Editor.Inspector;

namespace UGFramework.Coroutine
{
    /**
     * --- DOC BEGIN ---
     * A group can contains multiple coroutines. Others can extend this class and add additional group flags.
     * --- DOC END ---
     */
    [Serializable]
    public class CoroutineGroup
    {
        [ShowInInspector]        
        public string Name { get; private set; }

       /**
        * --- DOC BEGIN ---
        * CoroutineGroup can only be allocated in current and derived class.
        * --- DOC END ---
        */
        protected CoroutineGroup(string name) { this.Name = name; }

        public override string ToString()
        {
            return this.Name;
        }

        public static readonly CoroutineGroup DEFAULT = new CoroutineGroup("Default");

        public static readonly CoroutineGroup UGFRAMEWORK = new CoroutineGroup("UGFramework");
    }
}