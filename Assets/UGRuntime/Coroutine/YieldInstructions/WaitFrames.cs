namespace UGFramework.UGCoroutine
{
    public class WaitFrames : YieldInstruction
    {
        public override Status Status
        {
            get
            {
                if (_frames >= this.Frames)
                    return Status.FINISH;

                return Status.RUNNING;
            }
        }

        public uint Frames { get; private set; }

        uint _frames = 0; 

        public WaitFrames(uint frames)
        {
            if (frames == 0)
            {
                throw new System.Exception("There is no meaning for waiting 0 frame, cause waiting 0 frame is same as waiting 1 frame!");
            }
            this.Frames = frames;
        }

        public override void LateUpdate()
        {
            base.LateUpdate();    
            _frames++;
        }
    }
}