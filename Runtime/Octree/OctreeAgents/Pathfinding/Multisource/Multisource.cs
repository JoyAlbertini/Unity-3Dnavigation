//+
using Octree.Agent.Statistics;
using Octree.OctreeGeneration;
using System.Collections.Generic;
using UnityEngine;

namespace Octree.Agent.Pathfinding
{
    public class Multisource
    {
        private readonly Astar astar;
        private readonly BasicThetaStar basicThetaStar;
        private readonly LazyThetaStar lazyThetaStar;
        private PostSmooth postPathSmooth;
        public int openSetSize;
        public HashSet<PriorityNode> closed { get; private set; }
        public float travelledDistance { get; private set; }
        public uint lineOfSightChecks { get; private set; }
        public Multisource()
        {
            astar = new Astar();
            basicThetaStar = new BasicThetaStar();
            postPathSmooth = new PostSmooth();
            lazyThetaStar = new LazyThetaStar();
        }

        private OctreeNode targetNode = null;
        private List<OctreeTarget> agents;
        private bool nearestOctant;
        private bool cleanStart = false;
        private bool foundAllPaths;
        private bool postSmooth = false;
        private bool statistics = false;
        private PFA algorithmType;
       
        public void setGlobalParamter(bool nearestOctant, bool cleanPath, bool postSmooth, bool statistics)
        {
            this.nearestOctant = nearestOctant;
            this.cleanStart = cleanPath;
            this.postSmooth = postSmooth;
            this.statistics = statistics;
        }
        public bool computePath(Vector3 target, List<OctreeTarget> agents, PFA algorithm, float G = 1, float H = 1)
        {
            targetNode = SingletonOctree.Instance.octree.NearestNeighbour(target);
            this.agents = agents;
            travelledDistance = 0;
            lineOfSightChecks = 0;
            openSetSize = 0; 
            foundAllPaths = true;
            algorithmType = algorithm;

            switch (algorithm)
            {
                case PFA.Dijkstra:
                    astar.Clear();
                    closed = ComputePaths(astar, 1, 0);
                    break;
                case PFA.BestFirstSearch:
                    sortAgentByDistance(targetNode, agents);
                    astar.Clear();
                    closed = ComputePaths(astar, 0, 1);
                    break;
                case PFA.Astar:
                    sortAgentByDistance(targetNode, agents);
                    astar.Clear();
                    closed = ComputePaths(astar, G, H);
                    break;
                case PFA.BasicThetaStar:
                    postSmooth = false;
                    sortAgentByDistance(targetNode, agents);
                    basicThetaStar.Clear();
                    closed = ComputePaths(basicThetaStar, G, H);
                    lineOfSightChecks =  basicThetaStar.LineOfSightChecks();
                    break;
                case PFA.LazyThetaStar:
                    postSmooth = false;
                    sortAgentByDistance(targetNode, agents);
                    lazyThetaStar.Clear();
                    closed = ComputePaths(lazyThetaStar, G, H);
                    lineOfSightChecks = lazyThetaStar.LineOfSightChecks();
                    break;
            }
            return foundAllPaths;
        }

        HashSet<OctreeNode> octreeTargets; 

        private HashSet<PriorityNode> ComputePaths(AbstractPathfinding algorithm, float G, float H)
        {
            octreeTargets = new HashSet<OctreeNode>();
            int closed = 1, open = 1;
            if (statistics)
            {
                (closed, open) = SceneStatistics.Instance.stats.dictionary[algorithmType];
            }

            foreach (OctreeTarget agent in agents)
            {
                
                if (nearestOctant)
                {
                    agent.setNearestOctant(SingletonOctree.Instance.octree.NearestNeighbour(agent.transform.position));
                }

                if (!octreeTargets.Contains(agent.getNearesOctant()))
                {

                    agent.notFound = false;
                    if (algorithm.Main(targetNode, agent.getNearesOctant(), G, H, closed, open, agent))
                    {
                        SetPath(agent, algorithm);
                    }
                    else
                    {
                        agent.notFound = true;
                        Debug.Log("Path Not found Important");
                        foundAllPaths = false;
                    }
                    octreeTargets.Add(agent.getNearesOctant());
                }
                else
                {
                    SetPath(agent, algorithm);
                }
            }
            openSetSize = algorithm.GetOpenSize();
            return algorithm.GetClosedSet();
        }


        private void SetPath(OctreeTarget agent, AbstractPathfinding algorithm)
        {
            List<PriorityNode> path = new List<PriorityNode>(algorithm.RetrievePath(targetNode, agent.getNearesOctant()));
         
            if (postSmooth)
            {
                path = PostSmoothing(path, agent);
            } else
            {
                travelledDistance += algorithm.TravelledDistance();
            }
            if (cleanStart)
            {
                CleanStart(path, agent);
            }
            agent.SetPath(path);
        }

        private float distanceToTarget(OctreeNode dest, OctreeTarget agent)
        {
            return Vector3.Distance(dest.position, agent.transform.position);
        }
        private void sortAgentByDistance(OctreeNode dest, List<OctreeTarget> agents)
        {
            agents.Sort((a1, a2) => distanceToTarget(dest, a1).CompareTo(distanceToTarget(dest, a2)));
        }

        private List<PriorityNode> PostSmoothing(List<PriorityNode> path, OctreeTarget agent)
        {
            path = postPathSmooth.Main(path, agent);
            travelledDistance += postPathSmooth.travelledDistance;
            lineOfSightChecks += postPathSmooth.lineOfSightChecks;
            return path;
        }

        private void CleanStart(List<PriorityNode> path, OctreeTarget agent)
        {
            if (path.Count > 1)
            {

                if (agent.LineOfSight(agent.transform.position, path[1].position))
                {
                    path[0] = new PriorityNode(agent.transform.position, null);
                }
            
                /*
                if (!agent.LineOfSight(path[path.Count - 2].position, dest))
                {
                    path[path.Count - 1] = new PriorityNode(dest, null);
                } */
            }
        }
    }
}