//+
using UnityEngine;
using Octree.OctreeGeneration;

namespace Octree.Agent.Pathfinding
{
    public class LazyThetaStar : Astar
    {
        public LazyThetaStar() : base() { }
        protected override void setVertex(PriorityNode currentNode)
        {
            if (!agent.LineOfSight(currentNode.parent.position, currentNode.position))
            {
                lineOfSightCheck++;
                PriorityNode minNode = null;
                float minimum = Mathf.Infinity;
                foreach (OctreeNode neighbourNode in currentNode.node.neighbourNodes) { 
                    PriorityNode n = neighbourNode.priorityNode;
                    if (closed.Contains(n))
                    {
                        float score = n.gScore + G(n, currentNode);
                        if (score < minimum)
                        {
                            minimum = score;
                            minNode = n; 
                        }
                    }
                }
                currentNode.parent = minNode;
                currentNode.gScore = minimum;
            }
        }

        protected override void UpdateNodePriority(PriorityNode currentNode, PriorityNode neighbourNode)
        {
            float gScoretmp = currentNode.parent.gScore + G(currentNode.parent, neighbourNode);

            if (gScoretmp < neighbourNode.gScore)
            {
                neighbourNode.gScore = gScoretmp;
                neighbourNode.parent = currentNode.parent;
                neighbourNode.fScore = F(currentNode);
                replaceOrAdd(neighbourNode);
            }
        }
    }

}