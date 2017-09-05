using System;
using System.Collections;
using UGFramework.Pool;
using UGFramework.UGEditor.Inspector;

namespace UGFramework.UGCoroutine
{
    public enum Status
    {
        RUNNING,
        BREAK,
        FINISH,
    }

    /**
     * --- DOC BEGIN ---
     * --- DOC END ---
     */
    [Serializable]
    public class Coroutine : IObject 
    {
        public CoroutineGroup Group { get; set; }
        public IEnumerator Routine { get; set; }

        public int CreatedFrameCount { get; private set; }

        [ShowInInspector]
        public Status Status { get; private set; }

        [ShowInInspector]
        public bool IsRunning { get { return this.Status == Status.RUNNING; } }

        bool _noRoutine = false;

        public Coroutine()
        {
            this.CreatedFrameCount = UnityEngine.Time.frameCount;
            this.Status = Status.RUNNING;
        }

        public void Alloc(params object[] args) 
        { 
            this.Group = args[0] as CoroutineGroup;
            this.Routine = args[1] as IEnumerator;    
            this.Next(); 
            this.UpdateStatus(); 
        }
        public void Dealloc() { this.Status = Status.RUNNING; _noRoutine = false; }

        void Next()
        {
            // LogManager.Instance.Debug("Routine is null({0}) frame({1})", this.Routine.Current == null, UnityEngine.Time.frameCount);
            _noRoutine = !this.Routine.MoveNext();
            if (_noRoutine)
                return;

            var routine = this.Routine.Current as YieldInstruction;
            if (routine == null)
                return;
            routine.Coroutine = this; 
        }

        void UpdateStatus()
        {
            if (_noRoutine)
            {
                Status = Status.FINISH;
                return;
            }

            var routine = this.Routine.Current as YieldInstruction;
            if (routine == null) 
            {
                Status = Status.BREAK;
                return;
            }

            Status = routine.Status;
        }

        public void LateUpdate()
        {
            if (!this.IsRunning)
                return;
            
            if (this.Routine.Current is YieldInstruction == false)
            {
                throw new Exception(string.Format("Unkown class({0}), coroutine must return YieldInstruction!", this.Routine.Current.GetType().FullName));
            }

            // LateUpdate routine
            var routine = this.Routine.Current as YieldInstruction;
            routine.LateUpdate();

            if (routine.Status == Status.FINISH)
                this.Next();

            this.UpdateStatus();
        }
    }
}