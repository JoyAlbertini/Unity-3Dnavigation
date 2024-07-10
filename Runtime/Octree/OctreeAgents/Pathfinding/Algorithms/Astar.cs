//+
using UnityEngine;
using System.Collections.Generic;
using Octree.OctreeGeneration;

namespace Octree.Agent.Pathfinding 
{
    public class Astar : AbstractPathfinding
    {
        protected HashSet<PriorityNode> closed;
        protected float travelledDistance;
        protected uint lineOfSightCheck;
        private PriorityNode goal;
        private float _G;
        private float _H;
        protected C5.IPriorityQueue<PriorityNode> open;
        protected OctreeTarget agent;
        protected bool rewardContour = false;
        protected float rewardContourScore = 1;

        public Astar(){}
        public override void Clear()
        {
            if (closed != null)
            {
                closed.Clear();
            }
            closed = null;
            open = null; 
            lineOfSightCheck = 0;
        }

        public override bool Main(OctreeNode startNode, OctreeNode goalNode, float G, float H, int closedSize, int openSize, OctreeTarget agent = null)
        {
            this.agent = agent;

            if (closed == null)
            {
                closed = new HashSet<PriorityNode>(closedSize);
            }

            if (open == null)
            {
                open = new C5.IntervalHeap<PriorityNode>(openSize);
            }
            return computePath(startNode, goalNode, G, H);
        }

        protected bool computePath(OctreeNode startNode, OctreeNode goalNode, float G, float H)
        {
            if (startNode == null || goalNode == null)
            {
                return false; 
            }

            this._G = G;
            this._H = H; 
            PriorityNode start = startNode.priorityNode;
            goal = goalNode.priorityNode;
            if (closed.Contains(goal) || open.Find(goal.reference, out _))
            {
                
                return true;
            }
           
            start.parent = start;
            start.gScore = 0;
            start.fScore = F(start);
            start.reference = null; 
            open.Add(ref start.reference, start);

            while (!open.IsEmpty)
            {
                PriorityNode currentNode = open.DeleteMin();
                setVertex(currentNode);
              
                if (currentNode == goal)
                {
                    return true;
                }
                closed.Add(currentNode);
            
                foreach (OctreeNode neighbourNode in currentNode.node.neighbourNodes)
                {
                    PriorityNode neighbourPriority = neighbourNode.priorityNode; 
                    if (!closed.Contains(neighbourPriority))
                    {
                        if (!open.Find(neighbourPriority.reference, out _))
                        {
                            neighbourPriority.gScore = Mathf.Infinity;
                            neighbourPriority.fScore = Mathf.Infinity; 
                            neighbourPriority.parent = null; 
                        }
                        UpdateNodePriority(currentNode, neighbourPriority);
                    }
                }
            }

            return false;
        }

        protected virtual void setVertex(PriorityNode node){}
        protected virtual void UpdateNodePriority(PriorityNode currentNode, PriorityNode neighbourNode)
        {

            float gScoreTmp = currentNode.gScore + G(currentNode, neighbourNode); 

            if (gScoreTmp < neighbourNode.gScore)
            {
                neighbourNode.gScore = gScoreTmp;
                neighbourNode.parent = currentNode;
                neighbourNode.fScore = F(currentNode); ;
                replaceOrAdd(neighbourNode);
            }
        }

        protected void replaceOrAdd(PriorityNode neighbourNode)
        {
            if (open.Find(neighbourNode.reference, out _))
            {
                open.Replace(neighbourNode.reference, neighbourNode);
            }
            else
            {
                neighbourNode.reference = null;
                open.Add(ref neighbourNode.reference, neighbourNode);
            }
        }

        protected float F(PriorityNode n)
        {
            return n.gScore + H(n);  
        }

        protected float H(PriorityNode n)
        {  
            return _H * Vector3.Distance(n.position, goal.position); 
        }

        protected float G(PriorityNode n1, PriorityNode n2)
        {
            return _G *  Vector3.Distance(n1.position, n2.position); 
        }
        public override List<PriorityNode> RetrievePath(OctreeNode startNode, OctreeNode goalNode)
        {
            travelledDistance = 0;
            PriorityNode start = startNode.priorityNode; 
            PriorityNode goal = goalNode.priorityNode;
            List<PriorityNode> path = new List<PriorityNode>();

            PriorityNode currentNode = goal;

            while (currentNode != start)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
                travelledDistance += Vector3.Distance(currentNode.position, currentNode.parent.position);
            }
            travelledDistance += Vector3.Distance(currentNode.position, start.position);
            path.Add(start);

            return path;
        }

        public override HashSet<PriorityNode> GetClosedSet()
        {
            return closed;
        }

        public override float TravelledDistance()
        {
            return travelledDistance;
        }

        public uint LineOfSightChecks()
        {
            return lineOfSightCheck;
        }

        public override int GetOpenSize()
        {
            return open.Count;
        }
    }
}
