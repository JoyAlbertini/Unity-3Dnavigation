//+
using UnityEngine;
using Octree.OctreeGeneration;
using Octree.OctreeAgents.Utils;

namespace Octree.Agent
{
    public class AgentNearestOctant 
    {
        public OctreeNode nearestOctant = null;

        private InsideOctant insideOctant = new InsideOctant();

        public void Update(Vector3 position)
       {
          
            if (SingletonOctree.Instance.octree != null)
            {
                if (nearestOctant == null)
                {
                    SetNearestOctant(position);
                }

                if (!insideOctant.Check(position))
                {
                    SetNearestOctant(position);
                }
            }
        }

        private void SetNearestOctant(Vector3 position)
        {
            nearestOctant = SingletonOctree.Instance.octree.NearestNeighbour(position);
            insideOctant.CalculateOctantSizes(nearestOctant);
        }

        public void DrawNearestNeighbour(Vector3 position)
        {
            Gizmos.color = Color.red;
            if (nearestOctant != null)
            {
                Gizmos.DrawLine(position, nearestOctant.position);
            }
        }
    }
}