using UnityEngine;
using System;
using Octree.OctreeGeneration;

namespace Octree.Agent.Pathfinding
{
    public class PriorityNode : IComparable<PriorityNode>
    {
        public float fScore;
        public float gScore;
        public C5.IPriorityQueueHandle<PriorityNode> reference;
        public PriorityNode parent;
        public Vector3 position { get; private set; }
        public OctreeNode node { get; private set; }

        public PriorityNode(Vector3 position, OctreeNode node)
        {
            this.position = position;
            this.node = node; 
        }

        public int CompareTo(PriorityNode b)
        {
            if (this.fScore > b.fScore)
            {
                return 1; 
            } else if (this.fScore == b.fScore)
            {
                return 0; 
            } else
            {
                return -1; 
            } 
        }

    }
}