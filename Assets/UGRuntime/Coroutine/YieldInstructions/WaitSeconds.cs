using UnityEngine;

namespace UGFramework.UGCoroutine
{
    /**
     * --- DOC BEGIN ---
     * Make coroutine continue executing after seconds.
     * --- DOC END ---
     */
    public class WaitSeconds : YieldInstruction
    {
        public override Status Status
        {
            get
            {
                if (_seconds >= this.Seconds)
                    return Status.FINISH;

                return Status.RUNNING;
            }
        }

        public float Seconds { get; private set; }
        float _seconds = 0;

        public WaitSeconds(float seconds)
        {
            this.Seconds = seconds;            
        }

        public override void LateUpdate()
        {
            base.LateUpdate();    
            _seconds += Time.deltaTime;
        }
    }
}