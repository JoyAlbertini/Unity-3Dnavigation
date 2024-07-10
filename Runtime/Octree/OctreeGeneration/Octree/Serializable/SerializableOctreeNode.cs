using System.Collections.Generic;
using UnityEngine;

namespace Octree.OctreeGeneration.Serailizable
{
    public class SerializableOctreeNode
    {
        public Vector3 position;
        public float size;
        public int depth;
        public bool hasValidDescendant; 
        public bool validNode;
        public List<string> childNodes;
        public string nodeID; 
        public List<string> neighbourNodes;
        public Collider[] collision;
    }
}