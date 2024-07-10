//+
using UnityEngine;
using Octree.GameUtils;
namespace Octree.Agent.Utils
{
    [RequireComponent(typeof(OctreeSource))]
    public class TargetInstantiator : MonoBehaviour
    {

        [SerializeField] private int startNrOfAgent = 30;
        [SerializeField]
        private GameObject target;
        private OctreeSource source; 
        private void Awake()
        {
            source = transform.GetComponent<OctreeSource>();
            AddMultipleTargets(startNrOfAgent);
        }

        public void AddOrDeleteTargets(int targets)
        {
            int inSource = source.targets.Count; 

            if (targets == inSource)
            {
                return;
            }

            if (targets > inSource)
            {
                AddMultipleTargets(targets - inSource); 
            } else if (targets < inSource)
            {
                DeleteMultipleTargets(inSource - targets);
            }
        }

        private void AddMultipleTargets(int number)
        {
            for (int i = 0; i < number; i++)
            {
                AddTarget();
            }
        }

        private void DeleteMultipleTargets(int number)
        {
            for (int i = 0; i < number; i++)
            {
                RemoveTarget();
            }
        }

        public void AddTarget()
        {
            var t = Instantiate(target, RandomPositionInOctree.randomPosition(), Quaternion.identity, GlobalGameParemters.hierahchyTarget).GetComponent<OctreeTarget>();
            source.targets.Add(t);
        }

        public void RemoveTarget()
        {
            if (source.targets.Count > 0)
            {
                GameObject tmp = source.targets[source.targets.Count - 1].gameObject;
                source.targets.RemoveAt(source.targets.Count - 1);
                Destroy(tmp);
            }
        }

    }
}