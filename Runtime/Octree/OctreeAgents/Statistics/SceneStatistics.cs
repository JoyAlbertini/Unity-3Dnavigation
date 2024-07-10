//+
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
// source singleton that find itself: https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
namespace Octree.Agent.Statistics
{
    [ExecuteAlways]
    public class SceneStatistics : MonoBehaviour
    {
        private static SceneStatistics instance = null;
        public static SceneStatistics Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (SceneStatistics)FindObjectOfType(typeof(SceneStatistics));
                }
                return instance;
            }
        }

        public SerializeStats stats; 
       
        private string getPath()
        {
            return Application.dataPath + "/Octree/OctreeAgents/Statistics/Data/" + SceneManager.GetActiveScene().name + "_stat.txt";
        }

        public void SaveStatistics()
        {
            string path = getPath(); 
            string json = JsonUtility.ToJson(stats);
            File.WriteAllText(path, json);
        }

        public bool LoadStatistics()
        {
            string json = File.ReadAllText(getPath());
            stats = (SerializeStats)JsonUtility.FromJson(json, typeof(SerializeStats));
            return stats.Present();
        }
    }
}