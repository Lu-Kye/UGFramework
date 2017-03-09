using System.Collections;
using UnityEngine;
using UGFramework.Pool;

namespace UGFramework.Coroutine
{
    public enum Status
    {
        NULL = 1 << 0,
        WAITING = 1 << 1,
        BREAK = 1 << 2,
        FINISH = 1 << 3,

        END = BREAK | FINISH,
    }

    public class State 
    {
        public Coroutine Coroutine { get; set; }
        public Status Status { get; private set; }

        public bool IsEnd { get { return ((int)this.Status & (int)Status.END) == 1; } }

        public void Dispose()
        {
            this.Status = Status.NULL;
        }

        public void LateUpdate()
        {
            if (IsEnd)
                return;

            bool iterEnd = this.Coroutine.Routine.MoveNext();
            if (iterEnd)
            {
                Status = Status.FINISH;
                return;
            }

            var routine = this.Coroutine.Routine.Current;
            if (routine == null) 
            {
                Status = Status.BREAK;
                return;
            }

            if (routine is WaitForSeconds 
                || routine is WaitForFixedUpdate
                )
            {
                Status = Status.WAITING;
                var waitForSeconds = routine as WaitForSeconds;
                
            }
        }
    }

    /**
     * --- DOC BEGIN ---
     * --- DOC END ---
     */
    public class Coroutine : IObject 
    {
        public CoroutineGroup Group { get; set; }
        public IEnumerator Routine { get; set; }
        public State State { get; private set; }

        public Coroutine(CoroutineGroup group, IEnumerator routine)
        {
            this.Group = group;
            this.Routine = routine;    
            this.State = new State();
        }

        public void LateUpdate()
        {
            if (this.State.IsEnd)
                return;
            this.Routine.MoveNext();
            this.State.LateUpdate();
        }

        public void Init() { this.State.LateUpdate(); }
        public void Dispose() { this.State.Dispose(); }
    }
}