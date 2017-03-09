namespace UGFramework.Coroutine
{
    public enum YieldStatus
    {
        NULL,
        WAITING,
        END,
    }

    /**
     * --- DOC BEGIN ---
     * Base class of yield classes, like *WaitSeconds*, *WaitWWW*
     * --- DOC END ---
     */
    public class YieldInstruction
    {
        public YieldStatus Status { get; protected set; }

        public void LateUpdate()
        {
            this.Status = YieldStatus.END;
        }
    }
}