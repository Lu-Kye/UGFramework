namespace UGFramework.Coroutine
{
    /**
     * --- DOC BEGIN ---
     * Base class of yield classes, like *WaitSeconds*, *WaitWWW*
     * --- DOC END ---
     */
    public abstract class YieldInstruction
    {
        public Coroutine Coroutine { get; set; }

        public virtual Status Status
        {
            get 
            { 
                return Status.NULL;
            }
        }

        public virtual void LateUpdate()
        {
        }
    }
}