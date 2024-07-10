//+
using System.Collections.Generic;
// cade implemented using Introduction to algorithm third edition page 595 BFS


namespace Octree.OctreeGeneration.Utils 
{
    public class BFS 
    {
        private Queue<OctreeNode> Q;
        public HashSet<OctreeNode> closed { get; private set; }
        private HashSet<OctreeNode> visited;
        public void Main(OctreeNode start, int maxSize)
        {
            Q = new Queue<OctreeNode>(maxSize);
            visited = new HashSet<OctreeNode>();
            closed = new HashSet<OctreeNode>();
            Q.Enqueue(start); 
            while (Q.Count > 0)
            {
                OctreeNode u = Q.Dequeue();
                foreach (OctreeNode neighbour in u.neighbourNodes)
                {
                    if (!closed.Contains(neighbour))
                    {
                        if (!visited.Contains(neighbour))
                        {
                            Q.Enqueue(neighbour);
                            visited.Add(neighbour);
                        }
                    }
                }

                closed.Add(u);
            }
        }

        public HashSet<OctreeNode> reachableNodes()
        {
            return closed;
        }


    }
}