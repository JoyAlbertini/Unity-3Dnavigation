//+
using System.Collections.Generic;
using Octree.OctreeGeneration; 

namespace Octree.Agent.Pathfinding
{
    public abstract class AbstractPathfinding 
    {
        public abstract void Clear();
        public abstract bool Main(OctreeNode startNode, OctreeNode goalNode, float G, float H, int openSize, int closedSize, OctreeTarget agent = null);
        public abstract HashSet<PriorityNode> GetClosedSet();
        public abstract int GetOpenSize();
        public abstract float TravelledDistance();
        public abstract List<PriorityNode> RetrievePath(OctreeNode startNode, OctreeNode goalNode); 

    }
}