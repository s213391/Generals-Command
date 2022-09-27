using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DS_Selection
{
    //Class that uses a GUI to display/interact with selected objects, can be on an empty or image gameobject
    public class GUISelected : MonoBehaviour
    {
        public static GUISelected instance;

        [SerializeField]
        private GameObject selectedIconPrefab;
        [SerializeField]
        private GridLayoutGroup selectedObjects; //a layout group that displays all selected objects

        private UIBehaviour[] UIElements; //every ui element on this gameobject
        private bool panelOpen; //whether the selected GUI panel is open


        //set up singleton
        public void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
                instance = this;
        }


        //set up and hide the panel
        public void Init()
        {
            selectedObjects = GetComponentInChildren<GridLayoutGroup>();
            UIElements = GetComponents<UIBehaviour>();

            //clear all pre-existing children
            for (int i = selectedObjects.transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(selectedObjects.transform.GetChild(i).gameObject);

            TogglePanel(false);
        }


        //open/close the panel
        private void TogglePanel(bool open)
        {
            panelOpen = open;
            foreach (MonoBehaviour mb in UIElements)
                mb.enabled = open;

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(open);
            }
        }


        //add a new sprite to the selected panel
        public void AddSelectedIcon(Selectable selectable)
        {
            if (!panelOpen && SelectionController.instance.selectedObjects.Count > 1)
                TogglePanel(true);

            Instantiate(selectedIconPrefab, selectedObjects.transform).GetComponent<SelectedIcon>().Init(selectable);
        }


        //remove a sprite from the selected panel
        public void RemoveSelectedIcon(Selectable selectable)
        {
            if (!selectedObjects)
                return;

            for (int i = 0; i < selectedObjects.transform.childCount; i++)
            {
                if (selectedObjects.transform.GetChild(i).GetComponent<SelectedIcon>().attachedSelectable == selectable)
                {
                    DestroyImmediate(selectedObjects.transform.GetChild(i).gameObject);
                    return;
                }
            }

            if (selectedObjects.transform.childCount == 0)
                TogglePanel(false);
        }


        //remove all sprites from the selected panel
        public void ClearSelectedIcons()
        {
            if (!selectedObjects)
                return;

            for (int i = selectedObjects.transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(selectedObjects.transform.GetChild(i).gameObject);

            TogglePanel(false);
        }
    }
}
