using UnityEngine;

namespace Octree.Utils
{
    public static class OctreeDebugLog 
    {
        public static void OctreeTargetLog(string msg)
        {
            Debug.Log("<color=orange>OctreeTarget: </color>" + msg);
        }
        public static void OctreeTargetLogPathNotFound()
        {
            Debug.Log("<color=orange>OctreeTarget: </color><color=red>Path not found for one or multiple agents</color>");
        }

        public static void OctreeTargetSight(string msg)
        {
            Debug.Log("<color=orange>OctreeTarget:</color><color=cyan> sight: </color>" + msg);
        }

        public static void OctreeTargetTravveldDistance(string msg)
        {
            Debug.Log("<color=orange>OctreeTarget:</color><color=yellow> travelled distance: </color>" + msg);
        }

        public static void OctreeSourceLog(string msg)
        {
            Debug.Log("<color=green>OctreeSource: </color>" + msg);
        }

        public static void OctreeTesterLog(string msg)
        {
            Debug.Log("<color=red>OctreeTester: </color>" + msg);
        }

    }
}