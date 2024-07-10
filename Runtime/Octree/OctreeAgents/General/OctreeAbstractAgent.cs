//+
using UnityEngine;
using Octree.OctreeGeneration;

namespace Octree.Agent
{
    [ExecuteAlways]
    public class OctreeAbstractAgent : MonoBehaviour
    { 
       
       protected AgentNearestOctant agentNearestOctant = new AgentNearestOctant();

        protected virtual void Update()
        {
            agentNearestOctant.Update(transform.position); 
        }

        public virtual void OnDrawGizmos()
        {
            agentNearestOctant.DrawNearestNeighbour(transform.position);
        }

        protected OctreeNode getNearestOctant()
        {
            return agentNearestOctant.nearestOctant;
        }
       
    }
}