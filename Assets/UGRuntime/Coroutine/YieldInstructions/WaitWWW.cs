using UnityEngine;

namespace UGFramework.UGCoroutine
{
    /**
     * --- DOC BEGIN ---
     * --- DOC END ---
     */
    public class WaitWWW : YieldInstruction
    {
        public WWW WWW { get; private set; }

        public override Status Status
        {
            get
            {
                if (WWW == null)
                    return Status.BREAK;

                if (WWW.isDone)
                    return Status.FINISH;
                
                return Status.RUNNING;
            }
        }

        public WaitWWW(WWW www) 
        {
            this.WWW = www;
        }
    }
}