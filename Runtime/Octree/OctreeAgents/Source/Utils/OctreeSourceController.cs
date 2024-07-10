//+
using UnityEngine;
using Octree.UI;
using Octree.Agent;
using Octree.GameUtils;



namespace Octree.OctreeAgents.Utils
{
    [RequireComponent(typeof(CharacterController))]
    public class OctreeSourceController : MonoBehaviour
    {
        [SerializeField] private float normalSpeed = 10f;
        [SerializeField] private float runSpeed = 13f; 
        [SerializeField] private float increseHeight = 1.2f;
        private bool lockedE = false;
        private bool lockedQ = false; 
        private int pressedE = 0;
        private int pressedQ = 0;
        private bool  lockedMouse = false;
        private float rotationY;
        private float rotationX_;
        private CharacterController characterController; 

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            characterController = transform.GetComponent<CharacterController>();
        }

        private void Update()
        {

            if (GlobalNavigationParameters.canMove)
            {

                if (!lockedMouse)
                {    
                    // code source mouse look and keys https://answers.unity.com/questions/1727914/first-person-controller-script-not-working.html

                    float horizontal = Input.GetAxis("Horizontal");
                    float vertical = Input.GetAxis("Vertical");
                    float lookX = Input.GetAxis("Mouse X") * GlobalGameParemters.MouseSensibility * Time.deltaTime;
                    float lookY = Input.GetAxis("Mouse Y") * GlobalGameParemters.MouseSensibility * Time.deltaTime;
                    rotationX_ += lookX;
                    rotationY -= lookY;
                    rotationY = Mathf.Clamp(rotationY, -90f, 90f);
                    transform.rotation = Quaternion.Euler(rotationY, rotationX_, 0);
                    // vertical increases the forward position (Z axis)
                    // horizontal increase the right position  (X axis)
                    Vector3 move = (transform.forward * vertical) + (transform.right * horizontal);
                   
                    if (Input.GetKey(KeyCode.Space))
                    {
                        move.y += increseHeight;
                    }
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        characterController.Move(move * runSpeed * Time.deltaTime);
                    }
                    else
                    {
                        characterController.Move(move * normalSpeed * Time.deltaTime);
                    }
                } 
             

                if (Input.GetKeyDown(KeyCode.E) && !lockedE)
                {
                    if (pressedE % 2 == 0)
                    {
                        lockedMouse = true;
                        lockedQ = true;
                        PathPlanningUI.Instance.SetActive(true);
                        Cursor.lockState = CursorLockMode.Confined;
                    }
                    else
                    {
                        lockedMouse = false;
                        lockedQ = false;
                        PathPlanningUI.Instance.SetActive(false);
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                    pressedE++;
                }

                if (Input.GetKeyDown(KeyCode.Q) && !lockedQ)
                {
                    if (pressedQ % 2 == 0)
                    {
                        lockedMouse = true;
                        lockedE = true;
                        OptionUI.Instance.SetActive(true);
                        Cursor.lockState = CursorLockMode.Confined;
                    }
                    else
                    {
                        lockedMouse = false;
                        lockedE = false;
                        OptionUI.Instance.SetActive(false);
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                    pressedQ++;
                }
            } 
            
        }
    }

}