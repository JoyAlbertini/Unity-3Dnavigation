//+
using UnityEngine;
using System.Collections.Generic;
using Octree.Utils;
using Octree.Agent.Pathfinding;

namespace Octree.Agent
{
    public class OctreeTarget : OctreeTester
    {
        [SerializeField] private float radiousAgent = 1;
        [Header("Debug")]
        [SerializeField] private bool drawDebugRay = false;
        private List<PriorityNode> path;
        private OctreeTargetMovement octreeAgentMovement = null;
        private LineRenderer lineRenderer;

        public bool notFound = false;
       
        public void OnEnable()
        {
            if (TryGetComponent(out OctreeTargetMovement hinge))
            {
                octreeAgentMovement = hinge;
            }
            lineRenderer = GetComponent<LineRenderer>();
            
        }
        public void OnDrawGizmos()
        {
            agentNearestOctant.DrawNearestNeighbour(transform.position);
            DrawSphere();
            DrawNotPFound();
        }

        protected void Update()
        {
            //agentNearestOctant.Update(transform.position);
        }

        public bool IsMoving()
        {
            return octreeAgentMovement.IsMoving();
        }

        public override void SetPath(List<PriorityNode> path)
        {
            if (path.Count == 0)
            {
                OctreeDebugLog.OctreeSourceLog("Path not found");
            }
            this.path = path; 

            if (octreeAgentMovement != null)
            {
                octreeAgentMovement.setPath(path);
            }
            DrawPath();
        }
        private void DrawPath()
        {
            SetWidthLineRender();
            if (path != null)
            {
                lineRenderer.positionCount = path.Count;
                for (int i = 0; i < path.Count; i++)
                {
                    lineRenderer.SetPosition(i,path[i].position);
                }
            }
        }
        private  void SetWidthLineRender()
        {
            lineRenderer.SetWidth(GlobalNavigationParameters.thicknessPathEdge, GlobalNavigationParameters.thicknessPathEdge);
        }

        private void DrawSphere()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radiousAgent); 
        }  

        private void DrawNotPFound()
        {
            if (notFound)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.position, 5);
            }
        }

    }
}