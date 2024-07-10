//+
using System;
using UnityEngine;
using Octree.Utils;
using System.Collections.Generic;
using Octree.Agent.Pathfinding;
using System.IO;
using Octree.OctreeGeneration;

namespace Octree.Agent.Tester
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SingleAlgorithmTester))]
    public class MultipleAlgorithmTester : MonoBehaviour
    {
        [SerializeField] private string world;
        [Button("Test All Algorithms", "Main")] public bool button_1;
        private OctreeSource t; 
        private SingleAlgorithmTester algorithmTester;
        List<string> lines; 

        private void Start()
        {
            t = GetComponent<OctreeSource>();
            algorithmTester = GetComponent<SingleAlgorithmTester>();
        }

        private bool calculateNearestOctant_old;
        private PFA algorithm_old;
        private float G_old;
        private float H_old;
        private bool cleanStart_old;
        private bool postSmooth_old;
        private bool debugLog_old;
        private bool drawClosedSet_old;

        private void oldValues()
        {
            calculateNearestOctant_old = t.calculateNearestOctant;
            algorithm_old = t.algorithm;
            G_old = t.G;
            H_old = t.H;
            cleanStart_old = t.cleanStart;
            postSmooth_old = t.postSmooth;
            debugLog_old = t.debugLog;
            drawClosedSet_old = t.drawClosedSet;
        }

        private void setOldValues()
        {
            t.calculateNearestOctant = calculateNearestOctant_old; 
            t.algorithm = algorithm_old ;
            t.G = G_old;
            t.H = H_old;
            t.cleanStart = cleanStart_old;
            t.postSmooth = postSmooth_old;
            t.debugLog = debugLog_old;
            t.drawClosedSet = drawClosedSet_old;
        }
        public void Main()
        {
            oldValues();
            t.G = 1;
            t.H = 1;
            t.postSmooth = false;
            algorithmTester.log = false;
            algorithmTester.SetParameters();
            testAllAlgorithms();
            setOldValues();
        }

        private void addEmptyLine()
        {
            lines.Add("");
        }
        private void addStats(string algorithm, string type, float timeToCompute, float travelledDistance, float lineOfSightChecks, float closedSetNodes)
        {
            lines.Add(type);
            lines.Add(algorithm + " & " + timeToCompute + " & " + travelledDistance + " & " + lineOfSightChecks + " & " + closedSetNodes + " & " + (timeToCompute / (1 / travelledDistance)) / 100 + "\\" +"\\"); 
        }

        string multi = "Multi";
        string single = "Single";

        float timeToCompute = 0;
        float travelledDistance = 0;
        float lineOfSightChecks = 0;
        float closedSetNodes = 0;

        private void ComputePathSingleMulti(string variant)
        {
            (timeToCompute, travelledDistance, lineOfSightChecks, closedSetNodes) = algorithmTester.TestAlgorithm(false);
            addStats(variant, single, timeToCompute, travelledDistance, lineOfSightChecks, closedSetNodes);
            addEmptyLine();
            (timeToCompute, travelledDistance, lineOfSightChecks, closedSetNodes) = algorithmTester.TestAlgorithm(true);
            addStats(variant, multi, timeToCompute, travelledDistance, lineOfSightChecks, closedSetNodes);
            addEmptyLine();
        }

        private void DijkStra(string variant) {
            t.algorithm = PFA.Dijkstra;
            ComputePathSingleMulti(variant);
        }

        private void BestFirstSearch(string variant)
        {
            t.algorithm = PFA.BestFirstSearch;
            ComputePathSingleMulti(variant);
        }

        private void Astar(string variant)
        {
            t.algorithm = PFA.Astar;
            ComputePathSingleMulti(variant);
        }

        private void ThetaStar(string variant)
        {
            t.algorithm = PFA.BasicThetaStar;
            ComputePathSingleMulti(variant);
        }

        private void LazyThetaStar(string variant)
        {
            t.algorithm = PFA.LazyThetaStar;
            ComputePathSingleMulti(variant);
        }

        private void testAllAlgorithms()
        {
            lines = new List<string>();
            lines.Add(world);
            lines.Add("Seeds: " +String.Join(", ", algorithmTester.seeds.ConvertAll(a => a.ToString())));
            lines.Add("Nodes: " + SingletonOctree.Instance.octree.graphNodes.Count);
            lines.Add("Max depth: " + SingletonOctree.Instance.octree.maxDepth);
            addEmptyLine();
            DijkStra("Dijkstra");
            BestFirstSearch("BestFirstSearch");
            Astar("A*");
            t.postSmooth = true;
            DijkStra("Dijkstra PS");
            BestFirstSearch("BestFirstSearch PS");
            Astar("A* PS");
            t.postSmooth = false;
            ThetaStar("Basic Theta*");
            LazyThetaStar("Lazy Theta*");
            saveData();
        }

        private void saveData()
        {
            string path = Application.dataPath + "/Octree/AlgorithmTests/" + world + ".txt";
            File.WriteAllLines(path, lines);
        }
            
            
          
    }
}