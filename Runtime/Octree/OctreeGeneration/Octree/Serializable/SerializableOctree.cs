using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Octree.OctreeGeneration.Serailizable
{
    [Serializable]
    public class SerializableOctree
    {

        public SerializableOctreeNode rootNode = null;
        public List<SerializableOctreeNode> graphNodes;
        public List<SerializableOctreeNode> collisionNodes;

        public SerializableOctree(OctreeNode rootNode, HashSet<OctreeNode> graphNodes, HashSet<OctreeNode> collisionNodes)
        {

        }
    }
}