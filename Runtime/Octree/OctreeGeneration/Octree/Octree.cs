//+
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Octree.OctreeGeneration.Utils;
using Octree.Agent.Pathfinding;
using Octree.Agent;

namespace Octree.OctreeGeneration
{
    public class Octree
    {
        public OctreeNode rootNode { get; private set; } = null;
        public readonly OctreeNearestOctant nearestNeighbour;
        private OctreeNodeReferences ONR;

        // Nodes of the dual graph
        public List<OctreeNode> graphNodes { get; private set; }
        public HashSet<OctreeNode> collisionNodes { get; private set; }
        public Dictionary<Collider, HashSet<PriorityNode>> collisionContourNode { get; private set; }
        public uint maxDepth = 0;
        public Octree(
            Vector3 position, float cellSize, uint minDepth, uint maxDepth, LayerMask obstacleMask, bool nonConvex, GameObject octreeCleanUp = null)
            {
            GlobalNavigationParameters.obstructionMask = obstacleMask;
            this.maxDepth = maxDepth; 
            ONR = new OctreeNodeReferences(minDepth, maxDepth);
            List<BitArray> rootEncoding = new List<BitArray>();
            BitArray root = new BitArray(new bool[3] { false, false, false });
            rootEncoding.Add(root);
            int startDepth = 1;
            rootNode = new OctreeNode(position, cellSize, startDepth, rootEncoding, ref ONR);
            nearestNeighbour = new OctreeNearestOctant(rootNode);
            graphNodes = new HashSet<OctreeNode>(ONR.graphNodes.Values).ToList();
            collisionNodes = new HashSet<OctreeNode>(ONR.collisionNodes.Values);
            collisionContourNode = ONR.collisionContourNode;
            GenerateNeighbours();
            GenerateContour();

            if (nonConvex)
            {
                BFS BFS = new BFS();
                BFS.Main(nearestNeighbour.Main(octreeCleanUp.transform.position), graphNodes.Count);
                HashSet<OctreeNode> exceptNodes = new HashSet<OctreeNode>(graphNodes.Except(BFS.closed));
                foreach (OctreeNode delNode in exceptNodes)
                {
                    delNode.validNode = false;
                }

                RecomputeHasValidChildren(rootNode);
                rootNode.hasValidDescendant = true;
                graphNodes = BFS.closed.ToList();
            }
            CleanUp(rootNode.childNodes);
            GlobalNavigationParameters.computed = false;
        }

        private bool RecomputeHasValidChildren(OctreeNode curr)
        {
            
            bool hasValidChild = false;
            foreach (OctreeNode child in curr.childNodes)
            {
                if (RecomputeHasValidChildren(child) || child.hasValidDescendant)
                { 
                    hasValidChild = true;
                }
            }
            curr.hasValidDescendant = hasValidChild;

            return curr.validNode;
        }

        private void CleanUp(List<OctreeNode> nodes)
        {
            if (nodes.Count > 0)
            {
                foreach(OctreeNode node in nodes.ToList())
                {
                    CleanUp(node.childNodes);
                    if (!node.validNode && !node.hasValidDescendant)
                    {
                        nodes.Remove(node);
                    }
                }
            }
        }

        public OctreeNode NearestNeighbour(Vector3 position)
        {
            return nearestNeighbour.Main(position); 
        }

        private void GenerateNeighbours()
        {
            foreach (OctreeNode node in graphNodes)
            {
                node.DefineNeighboursNode(true);
            }
        }

        private void GenerateContour()
        {
            foreach (OctreeNode node in collisionNodes)
            {
                node.DefineNeighboursNode(false);
            }
        }
    }
}