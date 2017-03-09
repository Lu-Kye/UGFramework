namespace UGFramework.Coroutine
{
    public class WaitFrames : YieldInstruction
    {
        public override Status Status
        {
            get
            {
                if (_frames >= this.Frames)
                    return Status.FINISH;

                return Status.WAITING;
            }
        }

        public uint Frames { get; private set; }

        uint _frames = 0; 

        public WaitFrames(uint frames)
        {
            this.Frames = frames;
        }

        public override void LateUpdate()
        {
            base.LateUpdate();    
            _frames++;
        }
    }
}