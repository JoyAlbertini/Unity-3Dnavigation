//+
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Octree.OctreeGeneration;
using Octree.Utils;
using Octree.Agent.Statistics;
namespace Octree.Agent.Tester
{
    [ExecuteInEditMode]
    public class SingleAlgorithmTester : MonoBehaviour
    {
        public List<int> seeds;
        [Header("Max Number of iteration")]
        public bool maxNumberOfIterations = false;
        [SerializeField] private uint nrOfIterations;
        [System.NonSerialized] public bool log;
        [System.NonSerialized] public bool statistics;
        [Button("Test Algorithm", "Main")] public bool button_1;
        private OctreeSource source;
        private List<OctreeTarget> targets;
        private bool calculateNearestOctant_old;
        private bool debugLog_old;
        private bool drawClosedSet_old;
        private bool perform = true;

        private void OldValues()
        {
            debugLog_old = source.debugLog;
            drawClosedSet_old = source.drawClosedSet;
            calculateNearestOctant_old = source.calculateNearestOctant;
        }

        private void SetOldValues()
        {
            source.debugLog = debugLog_old;
            source.drawClosedSet = drawClosedSet_old;
            source.calculateNearestOctant = calculateNearestOctant_old;
        }

        public void SetParameters()
        {
            source = transform.GetComponent<OctreeSource>();
            targets = new List<OctreeTarget>();
            foreach (OctreeTarget src in source.targets)
            {
                OctreeTester tmp = src.GetComponent<OctreeTester>();
                if (tmp == null)
                {
                    perform = false;
                    OctreeDebugLog.OctreeTesterLog("All sources need an OctreeTester");
                }
                targets.Add((OctreeTarget) tmp);
            }
            source.debugLog = false;
            source.drawClosedSet = false;
            source.cleanStart = false;
            source.calculateNearestOctant = false;
        }

        public void Main()
        {
            log = true;
            SetParameters();
            OldValues();
            TestAlgorithm(true);
            SetOldValues();
        }

        public (float,float,float,float) TestAlgorithm(bool multi)
        {
            float travelledDistance = 0;
            float closedSetNodes = 0;
            float openSetNodes = 0;
            float lineOfSightChecks = 0;
            float timeToCompute = 0;
            int calls = 0;
            if (source.targets.Count > 0 && perform)
            {
                foreach (int seed in seeds)
                {
                    System.Random random = new System.Random(seed);
                    List<OctreeNode> positions = new List<OctreeNode>(SingletonOctree.Instance.octree.graphNodes);
                    List<OctreeNode> randomized = positions.OrderBy(x => random.Next()).ToList();
                    int i = 0;
                    for (i = 0; i + targets.Count < positions.Count; i += targets.Count)
                    {
                        if (maxNumberOfIterations)
                        {
                            if (i > nrOfIterations)
                            {
                                break;
                            }
                        }
                        int k = i;
                        foreach (OctreeTester agent in targets)
                        {
                            agent.setNearestOctant(randomized[k]);
                            k++;
                        }

                        if (multi)
                        {
                            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
                            source.MultiCalculatePath(targets);
                            stopWatch.Stop();
                            timeToCompute += stopWatch.ElapsedMilliseconds;
                            lineOfSightChecks += source.octreePath.LineOfSightChecks();
                            travelledDistance += source.octreePath.TravelledDistance();
                            closedSetNodes += source.octreePath.ClosedSet().Count();
                            openSetNodes += source.octreePath.OpenSetSize();
                        }
                        else
                        {
                            foreach (OctreeTarget src in targets)
                            {
                                System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
                                source.SingleCalculatePath(src);
                                stopWatch.Stop();
                                timeToCompute += stopWatch.ElapsedMilliseconds;
                                lineOfSightChecks += source.octreePath.LineOfSightChecks();
                                travelledDistance += source.octreePath.TravelledDistance();
                                closedSetNodes += source.octreePath.ClosedSet().Count();
                                openSetNodes += source.octreePath.OpenSetSize();
                            }
                        }
                    }
                    calls += i;
                }
                travelledDistance = travelledDistance / calls;
                closedSetNodes = closedSetNodes / calls;
                openSetNodes = openSetNodes / calls;
                timeToCompute = timeToCompute / calls;
                lineOfSightChecks = lineOfSightChecks / calls;
                if (log)
                {
                    OctreeDebugLog.OctreeTargetLog("Average distance: " + travelledDistance);
                    OctreeDebugLog.OctreeTargetLog("Average nodes: " + closedSetNodes);
                    OctreeDebugLog.OctreeTargetLog("Average time: " + timeToCompute);
                    OctreeDebugLog.OctreeTargetLog("Average line of sight: " + lineOfSightChecks);
                }
                if (statistics) {
                    SceneStatistics.Instance.stats.dictionary[source.algorithm] = ((int)closedSetNodes, (int)openSetNodes);
                }
            }
            return (timeToCompute, travelledDistance, lineOfSightChecks, closedSetNodes);
        }

    }
}