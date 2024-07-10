//+
using UnityEngine;
using Octree.Agent.Pathfinding;
using System.Collections.Generic;
using Octree.OctreeGeneration;

namespace Octree.Agent
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(OctreeTarget))]
    public class OctreeTester : MonoBehaviour
    {
        private LayerMask hitmaks1;
        private float radiousAgent = 1;
        protected AgentNearestOctant agentNearestOctant = new AgentNearestOctant();
        public bool path2 = false;

        private void Start()
        {
            hitmaks1 = GlobalNavigationParameters.obstructionMask;
        }

        public virtual void Initialize()
        {
            radiousAgent = transform.GetComponent<OctreeTarget>().radiousAgent; ;
        }

        public OctreeNode getNearesOctant()
        {
            return agentNearestOctant.nearestOctant;
        }
        public void setNearestOctant(OctreeNode node)
        {
            agentNearestOctant.nearestOctant = node;
        }

        public virtual void SetPath(List<PriorityNode> path) { }


        public bool LineOfSight(Vector3 currentNode, Vector3 neighbourNode)
        {
            Vector3 direction = neighbourNode - currentNode;
            float maxDistance = Vector3.Distance(neighbourNode, currentNode);
            return !Physics.SphereCast(currentNode, radiousAgent, direction, out _, maxDistance, hitmaks1);

        }
    }
}