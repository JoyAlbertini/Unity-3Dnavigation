using UnityEditor;
using UnityEngine;

namespace Octree.Utils
{
    
    public static class GizmoUtils 
    {
        public static void thickLine(Vector3 start, Vector3 finish, Color color, float thickness)
        {
        #if UNITY_EDITOR
            Handles.DrawBezier(start, finish, start, finish, color, null, thickness);
        #endif
        }
    }
}