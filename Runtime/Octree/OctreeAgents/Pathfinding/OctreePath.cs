//+
using UnityEngine;
using System.Collections.Generic;

namespace Octree.Agent.Pathfinding
{
    public enum PFA { Dijkstra, BestFirstSearch, Astar, BasicThetaStar, LazyThetaStar }

    public class OctreePath  
    {
        private readonly Multisource multisource;
        public OctreePath()
        {
            multisource = new Multisource();
        }
        public void setOptionalParamter(bool nearestOctant, bool cleanPath, bool postSmooth, bool statistic)
        {
            multisource.setGlobalParamter(nearestOctant, cleanPath, postSmooth, statistic);
        }

        public bool MultiDijkstra(Vector3 target, List<OctreeTarget> agents)
        {
            return multisource.computePath(target, agents, PFA.Dijkstra);
        }

        public bool MultiBestFirstSearch(Vector3 target, List<OctreeTarget> agents)
        {
            return multisource.computePath(target, agents, PFA.BestFirstSearch);
        }

        public bool MultiAstar(Vector3 target, List<OctreeTarget> agents, float G = 1, float H = 1)
        {
            return multisource.computePath(target, agents, PFA.Astar, G, H);
        }

        public bool MultiBasicThetaStar(Vector3 target, List<OctreeTarget> agents, float G = 1, float H = 1)
        {
            return multisource.computePath(target, agents, PFA.BasicThetaStar, G, H);
        }

        public bool MultiLazyThetaStar(Vector3 target, List<OctreeTarget> agents, float G = 1, float H = 1)
        {
            return multisource.computePath(target, agents, PFA.LazyThetaStar, G, H);
        }

        public HashSet<PriorityNode> ClosedSet()
        {
            return multisource.closed; 
        }

        public float TravelledDistance()
        {
            return multisource.travelledDistance;
        }

        public uint LineOfSightChecks()
        {
            return multisource.lineOfSightChecks;
        }

        public int OpenSetSize()
        {
            return multisource.openSetSize;
        }

    }
}