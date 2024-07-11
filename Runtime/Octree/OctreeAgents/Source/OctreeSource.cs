//+
using System.Collections.Generic;
using UnityEngine;
using Octree.Utils;
using Octree.OctreeGeneration;
using Octree.Agent.Pathfinding;
using Octree.Agent.Tester;
using Octree.UI;
using Octree.Agent.Utils;
using Octree.Agent.Statistics;
using System.Collections;

namespace Octree.Agent
{
    public enum MultiSingle { Multi, Single }
    [ExecuteAlways]
    [RequireComponent(typeof(OctreeSource))]
    public class OctreeSource : OctreeAbstractAgent
    {
        public List<OctreeTarget> targets;
        [Header("Solver")]
        public MultiSingle multiSingle;
        public PFA algorithm;
        public float G = 1;
        public float H = 1;
        [Header("Source")]
        [SerializeField] private bool continuosPathCalculation;
        [SerializeField] private float minimumTimeToRecompute;
        [Header("Targets")]
        public bool calculateNearestOctant = true;
        [Header("Path Option")]
        public bool cleanStart;
        [Header("A*, Best-First-Search, Dijkstra")]
        public bool postSmooth;
        [Header("Use Statistic")]
        public bool statistics;
        [Header("Multi-target-movement")]
        public bool replan = true;
        [Header("Debug")]
        public bool debugLog = true;
        public bool drawClosedSet = false;
        [SerializeField] private float radiousClosedSet = 1.5f;
        [Space(3)]
        [Button("Calculate path", "calculatePath")] public bool button_1;
        [Button("Calculate statistics", "CalculateStats")] public bool button_2;
        private OctreeNode prevNearestNeighbour;
        public OctreePath octreePath { get; private set; } = new OctreePath();
        private SingleAlgorithmTester algorithmTester;
        private SourceReplanner replanner;
        public GameObject source; 
       
        private void OnEnable()
        {
            replanner = transform.GetComponent<SourceReplanner>();
            calculateNearestOctant = true;
            algorithmTester = transform.GetComponent<SingleAlgorithmTester>();
        }

        private float time = 0;

#if UNITY_EDITOR
        private void OnValidate()
        {
            //CalculatePathAllTargets();
        }
#endif
      
        protected override void Update()
        {
            base.Update();
            time += Time.deltaTime;
            if (continuosPathCalculation)
            {

                if (prevNearestNeighbour != getNearestOctant())
                {
                    source.transform.position = getNearestOctant().position;
                   
                    if (replan)
                    {
                        replanner.StopReplanning();
                    }
                    time = 0;
                    CalculatePathAllTargets();
                    if (replan)
                    {
                        replanner.ActiveReplanning(getNearestOctant());
                    }
                }
                prevNearestNeighbour = getNearestOctant();
            }
        }

        public void SetParamters(MultiSingle multiSingle, PFA algorithm, float G, float H, bool cleanStart, bool postSmooth, bool statistics, bool replan)
        {
            this.multiSingle = multiSingle; 
            this.algorithm = algorithm;
            this.G = G;
            this.H = H;
            this.cleanStart = cleanStart;
            this.postSmooth = postSmooth;
            this.statistics = statistics;
            this.replan = replan;
         
            if (!replan)
            {
                replanner.StopReplanning();  
            }
            CalculatePathAllTargets();

        }

        private float timeToCompute;
        private int closedSet;
        private float travelledDistance;
        private uint lineOfSight;
        private bool foundPath; 

        public void CalculateStats()
        {
            calculateStatistic();
        }

        private void ResetValues()
        {
            timeToCompute = 0;
            closedSet = 0;
            travelledDistance = 0;
            lineOfSight = 0;
            foundPath = true;
        }

        private void DebugValues()
        {
            if (debugLog)
            {
                OctreeDebugLog.OctreeTargetLog("Time: " + timeToCompute);
                OctreeDebugLog.OctreeTargetLog("nodes in closed: " + closedSet);
                OctreeDebugLog.OctreeTargetTravveldDistance(travelledDistance.ToString());
                OctreeDebugLog.OctreeTargetSight(lineOfSight.ToString());

                if (foundPath)
                {
                    OctreeDebugLog.OctreeTargetLogPathNotFound();
                }
            }
        }

        public void CalculatePathAllTargets()
        {
            CalculatePath(targets);
        }

        public void CalculatePath(List<OctreeTarget> targets)
        {
            if (statistics)
            {

                if (!SceneStatistics.Instance.stats.Present())
                {
                    if (!SceneStatistics.Instance.LoadStatistics())
                    {
                         OctreeDebugLog.OctreeTargetLog("Need to compute statistic");
                         statistics = false;
                    }
                }
            }

            if (targets.Count > 0)
            {
                ResetValues();
                if (multiSingle == MultiSingle.Multi)
                {
                    MultiCalculatePath(targets);
                } else
                {
                    foreach (OctreeTarget target in targets)
                    {
                        SingleCalculatePath(target);
                    }
                }
                if (Application.isPlaying && PathPlanningUI.Instance != null)
                {

                    PathStatisticsUI.Instance.SetData(timeToCompute,travelledDistance, lineOfSight, closedSet);
                }
                DebugValues();
            }
        }

        System.Diagnostics.Stopwatch stopWatch;


        public void MultiCalculatePath(List<OctreeTarget> targets)
        {
            Vector3 pos = transform.position;
            octreePath.setOptionalParamter(calculateNearestOctant, cleanStart, postSmooth, statistics);

            stopWatch = System.Diagnostics.Stopwatch.StartNew();
            switch (algorithm)
            {
                case PFA.Dijkstra:
                    foundPath = !octreePath.MultiDijkstra(pos, targets);
                    break;
                case PFA.BestFirstSearch:
                    foundPath = !octreePath.MultiBestFirstSearch(pos, targets);
                    break;
                case PFA.Astar:
                    foundPath = !octreePath.MultiAstar(pos, targets, G, H);
                    break;
                case PFA.BasicThetaStar:
                    foundPath = !octreePath.MultiBasicThetaStar(pos, targets, G, H);
                    break;
                case PFA.LazyThetaStar:
                    foundPath = !octreePath.MultiLazyThetaStar(pos, targets, G, H);
                    break;
            }
            stopWatch.Stop();
            timeToCompute += stopWatch.ElapsedMilliseconds;
            travelledDistance += octreePath.TravelledDistance();
            lineOfSight += octreePath.LineOfSightChecks(); 
            closedSet += octreePath.ClosedSet().Count;
        }

        public void SingleCalculatePath(OctreeTarget source)
        {
            List<OctreeTarget> sources = new List<OctreeTarget>();
            sources.Add(source);
            MultiCalculatePath(sources);
        }

        public void ComputeShuffle()
        {
            StartCoroutine(shuffleTargets());
        }

        private IEnumerator shuffleTargets()
        {
            if (replan)
            {
                replanner.StopReplanning();
            }
            List<Coroutine> running = new List<Coroutine>();

            foreach (OctreeTarget target in targets)
            {
                var movement = target.GetComponent<OctreeTargetMovement>();
                movement.enabled = false;
                running.Add(StartCoroutine(MoveToPosition(target, movement)));
            }

            foreach(Coroutine c in running)
            {
                yield return c;
            }
            CalculatePathAllTargets();
         
        }

        private IEnumerator MoveToPosition(OctreeTarget target, OctreeTargetMovement movement)
        {
            yield return new WaitForSeconds(0.3f);
            target.transform.position = RandomPositionInOctree.randomPosition();
            movement.enabled = true;

        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            drawNodes();
        }

        public void drawNodes()
        {
            if (drawClosedSet)
            {
                if (octreePath.ClosedSet() != null)
                {
                    foreach (PriorityNode node in octreePath.ClosedSet())
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(node.position, radiousClosedSet);
                    }
                }
            }
        }

        private bool calculateNearestOctant_old;
        private MultiSingle multiSingle_old; 
        private PFA algorithm_old;
        private bool cleanStart_old;
        private bool postSmooth_old;
        private float G_old;
        private float H_old;
        private bool debugLog_old;
        private bool drawClosedSet_old;
        private bool algorithmTesterLog_old;

        private void oldValues()
        {
            calculateNearestOctant_old = calculateNearestOctant;
            multiSingle_old = multiSingle;
            algorithm_old = algorithm;
            cleanStart_old = cleanStart;
            postSmooth_old = postSmooth;
            debugLog_old = debugLog;
            G_old = G;
            H_old = H;
            drawClosedSet_old = drawClosedSet;

            if (algorithmTester != null)
            {
                algorithmTester.log = false;
                algorithmTester.statistics = true;
            }
        }

        private void setOldValues()
        {
            calculateNearestOctant = calculateNearestOctant_old;
            algorithm = algorithm_old;
            multiSingle = multiSingle_old;
            cleanStart = cleanStart_old;
            postSmooth = postSmooth_old;
            debugLog = debugLog_old;
            G = G_old;
            H = H_old;
            drawClosedSet = drawClosedSet_old;
            if (algorithmTester != null)
            {
                algorithmTester.log = false;
                algorithmTester.statistics = false;
            }
        }

        public void calculateStatistic()
        {
            oldValues();
            postSmooth = false;
            calculateNearestOctant = false;
            multiSingle = MultiSingle.Multi;
            debugLog = false;
            drawClosedSet = false;
            cleanStart = false;
            G = 1;
            H = 1;
            algorithmTester.SetParameters();
            algorithm = PFA.Dijkstra;
            algorithmTester.TestAlgorithm(false);
            algorithm = PFA.BestFirstSearch;
            algorithmTester.TestAlgorithm(false);
            algorithm = PFA.Astar;
            algorithmTester.TestAlgorithm(false);
            algorithm = PFA.BasicThetaStar;
            algorithmTester.TestAlgorithm(false);
            algorithm = PFA.LazyThetaStar;
            algorithmTester.TestAlgorithm(false);
            setOldValues();
            SceneStatistics.Instance.SaveStatistics();
        }


    }

}
