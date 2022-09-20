using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSModularSystem
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController instance { get; private set; }
        
        [Header("Position")]
        [Tooltip("Target object for the camera, and what the camera controller movement options actually move.\nCan be swapped to change the camera view between different positions and even follow moving objects")]
        public GameObject target;
        [Tooltip("Whether the target can be moved by the camera controller, or if the camera will simply follow passively")]
        public bool canMoveTarget = true;
        [Tooltip("Default distance camera sits behind the target in metres")]
        public float defaultDistance = 5.0f;
        [Tooltip("The current distance the camera is sitting behind the target in unity units")]
        public float currentDistance = 5.0f;
        [Tooltip("The X-axis angle the camera faces down at, recommended between 30 and 90")]
        public float xAngle = 45.0f;

        [Header("Zoom")]
        public float zoomSpeed = 2.0f;
        [Range(0.1f, 200.0f)] [Tooltip("The minimum distance from target object the camera can be. \nIncrease if camera clips through ground")]
        public float minZoom = 2.0f;
        [Range(0.1f, 200.0f)] [Tooltip("The maximum distance from target object the camera can be. \nDecrease if world disappears or mouse tracking stops working")]
        public float maxZoom = 10.0f;
        [Tooltip("How much of an effect that zooming out has on speeding up camera movement. \n0 or less will disable this feature")]
        public float speedUpOnZoomOut = 0.2f;

        [Header("Movement")]
        public float movementSpeed = 0.7f;
        [Tooltip("Uses Horizontal and Vertical axes set in Input Manager.\nIncludes WASD, Arrows and Joysticks by default.")]
        public bool moveWithAxes;
        [Tooltip("Uses the mouse position as well as a safe zone and a read zone to determine how fast to move the camera.")]
        public bool moveWithMouse;
        [Tooltip("Uses touch screen inputs to move the camera. \nSingle finger to move, Double finger pinch to zoom.")]
        public bool moveWithTouch;
        [ConditionalHide("moveWithTouch", "true"), Tooltip("How many pixels of movement must be detected before moving the camera")]
        public float minTouchDragDistance = 0.05f;

        [ConditionalHide("moveWithMouse", "true"), Range(0f, 1f), Tooltip("The percentage width of the screen that moving the mouse in won't move the camera sideways. \nValue of 1 will only move camera if mouse is outside of window")]
        public float xSafeZone = 0.6f;
        [ConditionalHide("moveWithMouse", "true"), Range(0f, 1f), Tooltip("The percentage height of the screen that moving the mouse in won't move the camera up or down. \nValue of 1 will only move camera if mouse is outside of window")]
        public float ySafeZone = 0.8f;
        [ConditionalHide("moveWithMouse", "true"), Range(0f, 1.5f), Tooltip("The percentage of the screen width that the mouse position will be tracked in.\nValues above 1 allow tracking outside of window")]
        public float xReadZone = 1.0f;
        [ConditionalHide("moveWithMouse", "true"), Range(0f, 1.5f), Tooltip("The percentage of the screen heigth that the mouse position will be tracked in.\nValues above 1 allow tracking outside of window")]
        public float yReadZone = 1.0f;
        [ConditionalHide("moveWithMouse", "true"), Tooltip("Whether moving the mouse outside of the read zone is considered movement at maximum speed(true) or no movement(false)")]
        public bool moveWhenOutsideOfReadZone;

        private bool movementEnabled = true; //whether the camera inputs are enabled
        private DeviceType device; //the type of device the game is running on


        //set up singleton
        void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
                instance = this;
        }


        //set the camera's initial position and angle
        public void Init(bool isHost)
        {
            //set default inputs based on device type
            device = SystemInfo.deviceType;
            if (device == DeviceType.Handheld)
            {
                moveWithTouch = true;
                moveWithAxes = false;
                moveWithMouse = false;
                movementEnabled = true;
            }
            
            currentDistance = defaultDistance;
            transform.eulerAngles = new Vector3(xAngle, 0.0f, 0.0f);
            canMoveTarget = true;

            //set target's initial position based on host status
            if (isHost)
                target.transform.position = ObjectDataManager.HostPosition();
            else
                target.transform.position = ObjectDataManager.ClientPosition();

            //prevent camera moving in opposite direction
            if (moveWithMouse && (xSafeZone >= xReadZone || ySafeZone >= yReadZone))
            {
                Debug.Log("Camera Controller: The mouse safe zone cannot be larger than the mouse read zone, Destroying controller");
                Destroy(this);
            }
        }

        //move the target and update the cameras position
        public void OnUpdate()
        {
            if (!movementEnabled)
                return;
            
            if (canMoveTarget)
            {
                float dx = 0.0f;
                float dz = 0.0f;

                //get the camera movement based on mouse position
                if (moveWithMouse)
                {
                    Vector3 mousePos = Input.mousePosition;
                    //offset the mouse position so that the centre of the screen is (0,0) and pos is between -1 and 1
                    mousePos.x -= Screen.width / 2;
                    mousePos.x /= Screen.width / 2;
                    mousePos.y -= Screen.height / 2;
                    mousePos.y /= Screen.height / 2;

                    //get the percentage value of how far out of the safe zone the mouse is horizontally
                    if (Mathf.Abs(mousePos.x) > xReadZone)
                    {
                        //mouse is outside of readzone, follow settings
                        if (moveWhenOutsideOfReadZone)
                        {
                            if (mousePos.x > 0)
                                dx += 1.0f;
                            else
                                dx -= 1.0f;
                        }
                    }
                    else if (Mathf.Abs(mousePos.x) > xSafeZone)
                    {
                        //mouse is in readzone but out of safe zone, set camera movement based on depth into non-safe readzone
                        if (mousePos.x > 0.0f)
                            mousePos.x -= xSafeZone;
                        else
                            mousePos.x += xSafeZone;
                        dx += mousePos.x / (xReadZone - xSafeZone);
                    }


                    //get the percentage value of how far out of the safe zone the mouse is vertically
                    if (Mathf.Abs(mousePos.y) > yReadZone)
                    {
                        //mouse is outside of readzone, follow settings
                        if (moveWhenOutsideOfReadZone)
                            if (moveWhenOutsideOfReadZone)
                            {
                                if (mousePos.y > 0)
                                    dz += 1.0f;
                                else
                                    dz -= 1.0f;
                            }
                    }
                    else if (Mathf.Abs(mousePos.y) > ySafeZone)
                    {
                        //mouse is in readzone but out of safe zone, set camera movement based on depth into non-safe readzone
                        if (mousePos.y > 0.0f)
                            mousePos.y -= ySafeZone;
                        else
                            mousePos.y += ySafeZone;
                        dz += mousePos.y / (yReadZone - ySafeZone);
                    }

                    //make sure mouse movements do not affect camera movement outside of readzone if not desired
                    if (!moveWhenOutsideOfReadZone && (Mathf.Abs(mousePos.x) > xReadZone || Mathf.Abs(mousePos.y) > yReadZone))
                    {
                        dx = 0.0f;
                        dz = 0.0f;
                    }
                }

                //get camera movement based on input manager axes
                if (moveWithAxes)
                {
                    dx += Input.GetAxisRaw("Horizontal");
                    dz += Input.GetAxisRaw("Vertical");
                }

                //get camera movement using single finger touch inputs
                if (moveWithTouch && Input.touchCount == 1)
                {
                    Touch finger = Input.GetTouch(0);
                    if (finger.phase == TouchPhase.Moved && finger.deltaPosition.magnitude >= minTouchDragDistance)
                    {
                        dx -= finger.deltaPosition.x / finger.deltaTime;
                        dz -= finger.deltaPosition.y / finger.deltaTime;
                    }
                }

                //clamp camera movement and update target position
                dx = Mathf.Clamp(dx, -1, 1);
                dz = Mathf.Clamp(dz, -1, 1);

                Transform targetTrans = target.transform;

                if (speedUpOnZoomOut > 0)
                    targetTrans.position += new Vector3(dx, 0.0f, dz) * movementSpeed * Time.deltaTime * currentDistance * speedUpOnZoomOut;
                else
                    targetTrans.position += new Vector3(dx, 0.0f, dz) * movementSpeed * Time.deltaTime;
            }

            // zoom in/out with mouse wheel or double finger pinching
            if (device == DeviceType.Desktop)
                currentDistance = Mathf.Clamp(currentDistance - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, minZoom, maxZoom);
            else if (moveWithTouch && Input.touchCount == 2)
            {
                Touch finger1 = Input.GetTouch(0);
                Touch finger2 = Input.GetTouch(1);

                //if two fingers are touching the screen and moving
                if (finger1.phase == TouchPhase.Moved || finger2.phase == TouchPhase.Moved)
                {
                    float fingerDistance = (finger1.position - finger2.position).magnitude;
                    float previousDistance = ((finger1.position - finger1.deltaPosition) - (finger2.position - finger2.deltaPosition)).magnitude;
                    float distanceChange = fingerDistance - previousDistance;

                    //if fingers are closer together, zoom out, else if further apart, zoom in
                    currentDistance = Mathf.Clamp(currentDistance - distanceChange * zoomSpeed * 0.1f, minZoom, maxZoom);
                }
            }

            // look at the target point 
            transform.position = target.transform.position - currentDistance * transform.forward;
        }


        //enables/disables camera movement
        public void ToggleCameraInputs(bool enabled)
        {
            movementEnabled = enabled;
        }
    }
}
