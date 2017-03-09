using System.Collections;
using System.Collections.Generic;
using UGFramework.Pool;

namespace UGFramework.Coroutine
{
    /**
     * --- DOC BEGIN ---
     * A manager of all coroutines at runtime.
     * --- DOC END ---
     */
    public class CoroutineManager : SingleTon<CoroutineManager> 
    {
        Dictionary<CoroutineGroup, LinkedList<Coroutine>> coroutineGroups = new Dictionary<CoroutineGroup,LinkedList<Coroutine>>();

        HashSet<Coroutine> removingCoroutines = new HashSet<Coroutine>();

        protected override void Ctor()
        {
            base.Ctor();
        }

        public Coroutine Run(IEnumerator routine, CoroutineGroup group = null)
        {
            if (group == null)
                group = CoroutineGroup.DEFAULT;

            LinkedList<Coroutine> coroutines;
            if (this.coroutineGroups.TryGetValue(group, out coroutines) == false)
            {
                coroutines = new LinkedList<Coroutine>();
                this.coroutineGroups[group] = coroutines;
            }

            // var coroutine = new Coroutine(group, routine);
            var coroutine = ObjectPool.Instance.Alloc<Coroutine>(group, routine);
            coroutines.AddLast(coroutine);

            return coroutine;
        }         

        public bool Stop(Coroutine coroutine)
        {
            return this.Remove(coroutine);
        }

        bool Remove(Coroutine coroutine)
        {
            if (this.removingCoroutines.Contains(coroutine))
                return false;

            this.removingCoroutines.Add(coroutine);
            return true;
        }

        /**
         * --- DOC BEGIN ---
         * Coroutine will be executed after *Update()*  
         * --- DOC END ---
         */
        void LateUpdate()
        {
            var coroutineGroupsIter = this.coroutineGroups.GetEnumerator();
            while (coroutineGroupsIter.MoveNext())
            {
                var coroutineGroupPair = coroutineGroupsIter.Current;
                var coroutines = coroutineGroupPair.Value;  

                var coroutineNode = coroutines.First;
                do {

                    var coroutine = coroutineNode.Value;
                    if (coroutine.State.IsEnd)
                    {
                        this.Remove(coroutine);
                        continue;
                    }
                    coroutine.LateUpdate();

                } while ((coroutineNode = coroutineNode.Next) != null);
            }
        }

        void Update()
        {
            // Remove finished coroutines
            if (this.removingCoroutines.Count <= 0)
                return;

            var removingCoroutinesIter = this.removingCoroutines.GetEnumerator();
            while (removingCoroutinesIter.MoveNext()) 
            {
                var removingCoroutine = removingCoroutinesIter.Current;
                var coroutineGroup = this.coroutineGroups[removingCoroutine.Group];

                coroutineGroup.Remove(removingCoroutine);

                // Deallocate the coroutine Instance
                ObjectPool.Instance.Dealloc(removingCoroutine);
            }
        }

    }
}