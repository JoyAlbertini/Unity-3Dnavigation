//+
using System;
using UnityEngine;
using System.Collections.Generic;
using Octree.Agent.Pathfinding;

namespace Octree.Agent.Statistics
{
    [Serializable]
    public class SerializeStats : ISerializationCallbackReceiver
    {
        public List<PFA> algorithms;
        public List<int> stats;
        public Dictionary<PFA, (int, int)> dictionary = new Dictionary<PFA, (int, int)>();

        public SerializeStats()
        {

        }

        public bool Present()
        {
            return dictionary.Keys.Count != 0;
        }
        public void OnBeforeSerialize()
        {
            if (dictionary != null)
            {
                algorithms = new List<PFA>(dictionary.Keys);
                stats = new List<int>();
                foreach ((int, int) val in dictionary.Values)
                {
                    stats.Add(val.Item1);
                    stats.Add(val.Item2);
                }
            }
        }

        public void OnAfterDeserialize()
        {
            dictionary.Clear();
            int i = 0;

            foreach (PFA algo in algorithms)
            {
                dictionary[algo] = (stats[i], stats[i + 1]);
                i += 2;
            }
            algorithms.Clear();
            stats.Clear();
        }
    }
}