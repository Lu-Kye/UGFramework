namespace UGFramework.Coroutine
{
    /**
     * --- DOC BEGIN ---
     * Make coroutine continue executing after seconds.
     * --- DOC END ---
     */
    public class WaitSeconds : YieldInstruction
    {
        public float Seconds { get; set; }
    }
}