using System.Collections;

namespace UGFramework.UGCoroutine
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
                _coroutine = CoroutinePool.Instance.Alloc<Coroutine>(new object[] { this.Coroutine.Group, _routine });
            }
            else if (_coroutine.IsRunning)
            {
                _coroutine.LateUpdate();
            }
        }
    }
}