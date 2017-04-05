using System.Collections;

namespace UGFramework.Coroutine
{
    public class WaitNewCoroutine : YieldInstruction
    {
        protected IEnumerator _routine;
        protected Coroutine _coroutine;

        public override Status Status
        {
            get
            {
                if (_coroutine == null || _coroutine.IsRunning)
                    return Status.RUNNING;
                return Status.FINISH;
            }
        }

        public WaitNewCoroutine(IEnumerator routine)
        {
            _routine = routine;
        }

        public override void LateUpdate()
        {
            if (_coroutine == null)
            {
                _coroutine = new Coroutine(this.Coroutine.Group, _routine);
                _coroutine.Init();
            }
            else if (_coroutine.IsRunning)
            {
                _coroutine.LateUpdate();
            }
        }
    }
}