using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using DS_Selection;

using Selectable = DS_Selection.Selectable;

namespace RTSModularSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UnitArrangement))]
    public class PlayerInput : MonoBehaviour
    {
        public static PlayerInput instance;
        
        [SerializeField]
        private RectTransform selectionBox;
        [SerializeField]
        private LayerMask objectLayers;
        [SerializeField]
        private LayerMask terrainLayers;
        [SerializeField]
        private float dragDelay = 0.1f;

        private Camera mainCam;
        private SelectionController selectionController;
        private UnitArrangement unitArrangement;
        private Image selectionImage;
        private float mouseDownTime;
        private Vector2 origMousePos;
        private Vector3 nullState = new Vector3(-99999, -99999, -99999);
        private DeviceType device;

        public Vector3 mouseWorldSpace { get; private set; }
        public PlayerObject objectUnderMouse { get; private set; }
        public Ray mouseRay { get; private set; }


        //set up singleton
        void Start()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
                instance = this;
        }


        //set up all variables and gameobjects required for getting player input
        public void Init()
        {
            mainCam = Camera.main;

            //set up object selection and movement orders
            if (!selectionBox)
                selectionBox = GameObject.FindGameObjectWithTag("Selection Box").GetComponent<RectTransform>();
            selectionImage = selectionBox.GetComponent<Image>();
            selectionImage.enabled = false;
            selectionController = RTSPlayer.selectionController;
            unitArrangement = GetComponent<UnitArrangement>();

            device = SystemInfo.deviceType;
        }


        //read and handle global player input while publicly exposing values for other scripts to use this frame
        public void OnUpdate()
        {
            if (device == DeviceType.Desktop)
            {
                CheckUnderMouse();
                HandleDesktopSelectionInputs();
                HandleDesktopMovementInputs();
            }
            else if (device == DeviceType.Handheld)
            { 
                //TBC// Create functions for mobile input
            }
        }


        //get information involving the mouse and anything under it and set it to publicly available
        private void CheckUnderMouse()
        {
            mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);

            //check the terrain to get the world space position of the mouse
            if (Physics.Raycast(mouseRay, out RaycastHit terrainHit, 50.0f, terrainLayers))
                mouseWorldSpace = terrainHit.point;
            else
                mouseWorldSpace = nullState;

            //check player objects to see if any are under the mouse
            if (Physics.Raycast(mouseRay, out RaycastHit objectHit, 50.0f, objectLayers))
                objectUnderMouse = objectHit.collider.GetComponentInParent<PlayerObject>();
            else
                objectUnderMouse = null;
        }


        //check if any selected objects need to be given a navmesh destination
        private void HandleDesktopMovementInputs()
        {
            if (Input.GetKeyUp(KeyCode.Mouse1) && selectionController.selectedObjects.Count > 0)
            {
                if (mouseWorldSpace != nullState)
                {
                    //add every selected movable object to a list
                    List<NavMeshAgent> moveables = new List<NavMeshAgent>();
                    foreach (Selectable selectable in selectionController.selectedObjects)
                    {
                        PlayerObject po = selectable.GetComponent<PlayerObject>();
                        if (po && po.data.moveable)
                            moveables.Add(po.GetComponent<NavMeshAgent>());
                    }

                    //check if an object is already at the clicked point and set it as the movement target
                    if (objectUnderMouse == null)
                        unitArrangement.AssignDestination(moveables, mouseWorldSpace);
                    else if (RTSPlayer.Owns(objectUnderMouse))
                        unitArrangement.AssignDestination(moveables, objectUnderMouse.transform.position, false, objectUnderMouse);
                    else
                        unitArrangement.AssignDestination(moveables, objectUnderMouse.transform.position, true, objectUnderMouse);
                }
            }
        }


        //handle the selection of player objects, either through single click or drag
        private void HandleDesktopSelectionInputs()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                selectionBox.sizeDelta = Vector2.zero;
                selectionImage.enabled = true;
                origMousePos = Input.mousePosition;
                mouseDownTime = 0.0f;
            }
            else if (Input.GetKey(KeyCode.Mouse0))
            {
                mouseDownTime += Time.deltaTime;
                if (mouseDownTime >= dragDelay)
                {
                    float width = Input.mousePosition.x - origMousePos.x;
                    float height = Input.mousePosition.y - origMousePos.y;

                    selectionBox.anchoredPosition = origMousePos + new Vector2(width / 2, height / 2);
                    selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
                }
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                bool shift = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
                //used to shorten the long name of this list
                List<Selectable> selectables = selectionController.availableObjects;

                //only check the box if the mouse has been down for a set delay
                if (mouseDownTime > dragDelay)
                {
                    //create a bounding box in screen space and select every owned player object in the world space of that box
                    Bounds bounds = new Bounds(selectionBox.anchoredPosition, selectionBox.sizeDelta);
                    for (int i = 0; i < selectables.Count; i++)
                    {
                        PlayerObject po = selectables[i].GetComponent<PlayerObject>();
                        if (po && RTSPlayer.Owns(po) && IsInSelectionBox(po, bounds))
                        {
                            if (selectionController.IsSelected(selectables[i]) && shift)
                                selectionController.Deselect(selectables[i]);
                            else
                                selectionController.Select(selectables[i]);
                        } 
                        else if (!shift)
                            selectionController.Deselect(selectables[i]);
                    }
                }
                //else treat as a single click
                else 
                {
                    if (objectUnderMouse != null && RTSPlayer.Owns(objectUnderMouse))
                    {
                        Selectable selectable = objectUnderMouse.GetComponent<Selectable>();

                        if (shift)
                        {
                            if (selectionController.IsSelected(selectable))
                                selectionController.Deselect(selectable);
                            else
                                selectionController.Select(selectable);
                        }
                        else
                        {
                            selectionController.DeselectAll();
                            selectionController.Select(selectable);
                        }
                    }
                    else if (!shift)
                        selectionController.DeselectAll();
                }

                //hide the selection box
                selectionBox.sizeDelta = Vector2.zero;
                selectionImage.enabled = false;

                mouseDownTime = 0.0f;
            }
        }


        //returns whether the given player object is within the given bounds
        private bool IsInSelectionBox(PlayerObject po, Bounds bounds)
        {
            Vector3 pos = mainCam.WorldToScreenPoint(po.transform.position);

            return pos.x > bounds.min.x && pos.x < bounds.max.x
                && pos.y > bounds.min.y && pos.y < bounds.max.y;
        }
    }
}