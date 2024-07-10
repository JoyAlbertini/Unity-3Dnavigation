//+
using UnityEngine;
using System.Collections.Generic;
using Octree.Agent.Pathfinding;

namespace Octree.Agent
{
    [RequireComponent(typeof(OctreeTarget))]
    public class OctreeTargetMovement : MonoBehaviour
    {
        [SerializeField] private bool move;
        public float speed;
        private List<PriorityNode> path = null;
        private int i = 0;
        private CharacterController CR;
        [SerializeField]
        private float rndSpeed;
        private Vector3 movement;
        private void Start()
        {
            rndSpeed = Random.Range(5, 18);
            speed = rndSpeed * GlobalNavigationParameters.globalSpeed;
            CR = GetComponent<CharacterController>();
        }

        private void Update()
        {
            speed = rndSpeed * GlobalNavigationParameters.globalSpeed;
            if (move)
            {
                if (path != null) {
                    if (i < path.Count)
                    {
                        Vector3 dest = path[i].position;
                        Vector3 direction = dest - transform.position;
                        movement = direction.normalized * speed * Time.deltaTime;
                        CR.Move(movement);

                        if (reachedTarget(dest))
                        {
                            i++;
                        }
                       
                    }
                }
            }
        }

        public bool IsMoving()
        {
            var moving = !CR.velocity.Equals(Vector3.zero);
            return moving;
        }

        private bool reachedTarget(Vector3 point)
        {
            return (Vector3.Distance(transform.position, point) < GlobalNavigationParameters.thresholdNearPoint);
        }

        public void setPath(List<PriorityNode> path)
        {
            this.path = path;
            i = 0;
        }
    }
}