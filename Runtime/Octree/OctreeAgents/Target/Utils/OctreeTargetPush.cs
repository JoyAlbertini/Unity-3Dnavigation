//+
using UnityEngine;



namespace Octree.Agent
{
    [RequireComponent(typeof(OctreeTargetMovement))]
    public class OctreeTargetPush : MonoBehaviour
    {

        [SerializeField] private float force;
        private Vector3 impact = Vector3.zero;
        public float speed { get; private set; }
        private CharacterController cr;
        private OctreeTargetMovement movement;
        void Start()
        {
            force = Random.Range(0.5f, 1.3f);
            cr = GetComponent<CharacterController>();
            movement = GetComponent<OctreeTargetMovement>();
            speed = movement.speed;
        }

        public void moveObject()
        {
            cr.Move(impact * Time.deltaTime);
        }

        private bool newImpact = false;


        //Code idea from: https://answers.unity.com/questions/502798/object-push-character-controller.html
        void Update()
        {
            if (newImpact)
            {
                if (impact.magnitude > 0.2f)
                {
                    moveObject();
                } else {
                    newImpact = false;
                }
                impact = Vector3.Lerp(impact, Vector3.zero, 5.2f * Time.deltaTime);
            }
        }

        public void AddImpactWithDirection(Vector3 pos1, Vector3 pos2, float force)
        {
            newImpact = true;
            Vector3 dir = pos2 - pos1;
            dir.Normalize() ;
            impact += (dir.normalized * force * speed)* GlobalNavigationParameters.globalCollisionForce;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.transform.TryGetComponent(out OctreeTargetPush hinge))
            {
                float offset = 30f;
                Vector3 slightRanHit = hit.point;
                slightRanHit.x = Random.Range(slightRanHit.x - offset, slightRanHit.x + offset);
                slightRanHit.y = Random.Range(slightRanHit.y - offset, slightRanHit.y + offset);
                slightRanHit.z = Random.Range(slightRanHit.z - offset, slightRanHit.z + offset);
                if (hinge.speed < speed)
                {
                    hinge.AddImpactWithDirection(transform.position, slightRanHit, force);
                } else if (hinge.speed == speed)
                {
                    int val = Random.Range(0, 1);
                    if (val == 0)
                    {
                        hinge.AddImpactWithDirection(transform.position, slightRanHit, force);
                    }
                }
            }
        }
    }
}