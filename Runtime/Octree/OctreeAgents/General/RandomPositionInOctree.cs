//+
using UnityEngine;
using Octree.OctreeGeneration;

namespace Octree.Agent.Utils
{
    public static class RandomPositionInOctree 
    {
        public static Vector3 randomPosition()
        {
            System.Random random = new System.Random();
            int r = random.Next(0, SingletonOctree.Instance.octree.graphNodes.Count);
            return SingletonOctree.Instance.octree.graphNodes[r].position;

        }
    }
}