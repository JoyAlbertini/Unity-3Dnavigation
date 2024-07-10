//+
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Octree.OctreeGeneration;
using Octree.OctreeAgents.Utils;

namespace Octree.Agent.Utils
{
    public class SourceReplanner : MonoBehaviour
    {
        private OctreeSource octreeSource;
        private InsideOctant insideOctant;
        private OctreeNode goal;
        private List<OctreeTarget> targetsToReplan;
        private void Start()
        {
            octreeSource = transform.GetComponent<OctreeSource>();
            insideOctant = new InsideOctant();
        }

        public void ActiveReplanning(OctreeNode goal)
        {
            if (Application.isPlaying)
            {
                this.goal = goal;
                StartCoroutine(Replan());
            }
        }

        public void StopReplanning()
        {
            if (Application.isPlaying)
            {
                StopAllCoroutines();
            }
        }

        private IEnumerator Replan()
        {
            while (true)
            {
                yield return new WaitForSeconds(GlobalNavigationParameters.replanTime);
                ReplanTargets();
                octreeSource.CalculatePath(targetsToReplan);
            }
            
        }

        private void ReplanTargets()
        {
            Debug.Log("replanner");
            targetsToReplan = new List<OctreeTarget>();
            insideOctant.CalculateOctantSizes(goal);

            foreach (OctreeTarget target in octreeSource.targets)
            {
                if (!insideOctant.Check(target.transform.position))
                {
                    if (!target.IsMoving())
                    {
                        targetsToReplan.Add(target);
                    }
                }
            }
            octreeSource.CalculatePath(targetsToReplan);
        }

        private void OnDrawGizmos()
        {
            if (targetsToReplan != null)
            {
                foreach (OctreeTarget target in targetsToReplan)
                {
                    if (target != null)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(target.transform.position, 6f);
                    }
                }
            }
        }

    }
}