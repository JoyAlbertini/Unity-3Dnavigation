//+
using UnityEngine;

namespace Octree.OctreeGeneration
{
    public class OctreeNearestOctant 
    {
        private OctreeNode root;

        public OctreeNearestOctant(OctreeNode root)
        {
            this.root = root;
        }

        public OctreeNode Main(Vector3 position)
        {
           
            if (!root.hasValidDescendant && !root.validNode)
            {
                return null;
            }
            else
            {
                OctreeNode neighbour = root;
                while (validDescendant(ref neighbour, position)){}
                return neighbour;
            }
        }
        private bool validDescendant(ref OctreeNode parent, Vector3 position)
        {
            if (parent == null)
            {
                return true;
            }

            OctreeNode minDistanceNode = null;
            float minDistance = Mathf.Infinity;
            foreach (OctreeNode child in parent.childNodes)
            {
                if (child != null)
                {
                    
                    float currentNodeDistance = Vector3.Distance(child.position, position);
                    if (currentNodeDistance < minDistance)
                    {
                        minDistanceNode = child;
                        minDistance = currentNodeDistance;
                    }
                }
            }

            parent = minDistanceNode;
            return (!minDistanceNode.validNode);
        }
    }
}