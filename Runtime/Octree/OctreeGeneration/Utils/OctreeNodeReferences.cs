//+
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Octree.Agent.Pathfinding;

namespace Octree.OctreeGeneration.Utils
{
    public class OctreeNodeReferences
    {
        private class listBitArrayComparer : IEqualityComparer<List<BitArray>>
        {
            public bool Equals(List<BitArray> a1, List<BitArray> a2)
            {
                if (a1.Count == a2.Count)
                {
                    bool eql = true;
                    for (int i = 0; i < a1.Count; i++)
                    {
                        if (a1[i].BitsToString() != a2[i].BitsToString())
                        {
                            eql = false;
                            break;
                        }
                    }
                    return eql;
                }
                else
                {
                    return false;
                }
            }
            public int GetHashCode(List<BitArray> a)
            {
                string tmp = "";
                foreach (BitArray arr in a)
                {
                    tmp += arr.BitsToString();
                }
                return tmp.GetHashCode();
            }
        }

        public Dictionary<List<BitArray>, OctreeNode> graphNodes;
        public Dictionary<List<BitArray>, OctreeNode> collisionNodes;
        public Dictionary<Collider, HashSet<PriorityNode>> collisionContourNode; 
        public uint maxDepth;
        public uint minDepth;

        public OctreeNodeReferences(uint minDepth, uint maxDepth)
        {
            graphNodes = new Dictionary<List<BitArray>, OctreeNode>(new listBitArrayComparer());
            collisionNodes = new Dictionary<List<BitArray>, OctreeNode>(new listBitArrayComparer());
            collisionContourNode = new Dictionary<Collider, HashSet<PriorityNode>>();
            this.minDepth = minDepth;
            this.maxDepth = maxDepth;
        }
      
    }
}
