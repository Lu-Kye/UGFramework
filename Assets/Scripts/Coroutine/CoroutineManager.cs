using System.Collections;
using System.Collections.Generic;

namespace UGFramework
{
    /**
     * --- DOC BEGIN ---
     * --- DOC END ---
     */
    public class CoroutineManager : SingleTon<CoroutineManager> 
    {
        Dictionary<CoroutineGroup, LinkedList<IEnumerator>> routineGroups = new Dictionary<CoroutineGroup,LinkedList<IEnumerator>>();
        IEnumerator rGEnumerator;

        protected override void Ctor()
        {
            base.Ctor();
            rGEnumerator = routineGroups.GetEnumerator();
        }

        public void Start(CoroutineGroup group, IEnumerator routine)
        {
            LinkedList<IEnumerator> routines;
            if (routineGroups.TryGetValue(group, out routines) == false)
            {
                routines = new LinkedList<IEnumerator>();
                routineGroups[group] = routines;
            }
            routines.AddLast(routine);
        }         

        void LateUpdate()
        {
        }
    }
}