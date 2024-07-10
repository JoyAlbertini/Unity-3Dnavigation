//+
using UnityEngine;
using Octree.OctreeGeneration;

namespace Octree.OctreeAgents.Utils
{
    public class InsideOctant
    {
        private float x_max;
        private float x_min;
        private float y_max;
        private float y_min;
        private float z_max;
        private float z_min;

        public void CalculateOctantSizes(OctreeNode node)
        {
            float nodeSize = node.size / 2;
            x_max = node.position.x + nodeSize;
            x_min = node.position.x - nodeSize;
            y_max = node.position.y + nodeSize;
            y_min = node.position.y - nodeSize;
            z_max = node.position.z + nodeSize;
            z_min = node.position.z - nodeSize;
        }
        public bool Check(Vector3 position)
        {
            return x_min <= position.x && position.x <= x_max &&
                   y_min <= position.y && position.y <= y_max &&
                   z_min <= position.z && position.z <= z_max;
        }
    }
}