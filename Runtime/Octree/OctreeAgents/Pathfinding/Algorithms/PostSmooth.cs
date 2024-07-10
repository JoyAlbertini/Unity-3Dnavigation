//+
using System.Collections.Generic;
using UnityEngine;

namespace Octree.Agent.Pathfinding
{
    public class PostSmooth 
    {
        public float travelledDistance { get; private set; } 
        public uint lineOfSightChecks { get; private set; }
        public List<PriorityNode> Main(List<PriorityNode> path, OctreeTarget agent)
        {
            lineOfSightChecks = 0;
            travelledDistance = 0;
            List<PriorityNode> smoothPath = new List<PriorityNode>();
            smoothPath.Add(path[0]);

            for (int i = 1; i < path.Count - 1; i++)
            {
                if (!agent.LineOfSight(smoothPath[smoothPath.Count - 1].position, path[i + 1].position))
                {
                    lineOfSightChecks++;
                    travelledDistance += Vector3.Distance(smoothPath[smoothPath.Count - 1].position, path[i].position); 
                    smoothPath.Add(path[i]);
                }
            }
            travelledDistance += Vector3.Distance(smoothPath[smoothPath.Count - 1].position, path[path.Count - 1].position);
            smoothPath.Add(path[path.Count - 1]);

            return smoothPath;
        }
    }
}