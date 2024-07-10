
using UnityEngine;

namespace Octree.Agent
{
    public static class GlobalNavigationParameters 
    {
        public static bool computed = false;
        public static float radiousPathVertex;
        public static float thicknessPathEdge;
        public static LayerMask obstructionMask;
        public static bool canMove = false;
        public static float globalSpeed = 1;
        public static float globalCollisionForce;
        public static float thresholdNearPoint;
        public static float replanTime; 

    }
}