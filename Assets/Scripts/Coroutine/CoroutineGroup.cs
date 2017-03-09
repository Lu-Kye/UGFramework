namespace UGFramework.Coroutine
{
    /**
     * --- DOC BEGIN ---
     * A group can contains multiple coroutines. Others can extend this class and add additional group flags.
     * --- DOC END ---
     */
    public class CoroutineGroup
    {
       /**
        * --- DOC BEGIN ---
        * CoroutineGroup can only be allocated in current and derived class.
        * --- DOC END ---
        */
        protected CoroutineGroup() {}

        public static readonly CoroutineGroup DEFAULT = new CoroutineGroup();

        public static readonly CoroutineGroup UGFRAMEWORK = new CoroutineGroup();
    }
}