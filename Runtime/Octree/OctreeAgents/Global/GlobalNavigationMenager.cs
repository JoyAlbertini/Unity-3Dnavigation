//+
using UnityEngine;
using Octree.GameUtils;

namespace Octree.Agent
{
    public class GlobalNavigationMenager : MonoBehaviour
    {
        private float radiousPathVertex = 1;
        [SerializeField]
        private float globalCollisionForce = 1;
        [SerializeField]
        private float thresholdNearPoint = 1;
        [SerializeField]
        private float replanTime = 0.30f;
        [SerializeField]
        private float thicknessPathEdge = 1;
        [SerializeField]
        private Transform hierachy;

        private void Awake()
        {
            GlobalGameParemters.hierahchyTarget = hierachy;
        }

        private void Start()
        {
            GlobalNavigationParameters.replanTime = replanTime;
            GlobalNavigationParameters.radiousPathVertex = radiousPathVertex;
            GlobalNavigationParameters.thicknessPathEdge = thicknessPathEdge;
            GlobalNavigationParameters.globalCollisionForce = globalCollisionForce;
            GlobalNavigationParameters.thresholdNearPoint = thresholdNearPoint;
        }

        private void OnValidate()
        {
            GlobalNavigationParameters.replanTime = replanTime;
            GlobalNavigationParameters.thresholdNearPoint = thresholdNearPoint;
            GlobalNavigationParameters.radiousPathVertex = radiousPathVertex;
            GlobalNavigationParameters.thicknessPathEdge = thicknessPathEdge;
            GlobalNavigationParameters.globalCollisionForce = globalCollisionForce;
        }
    }
}