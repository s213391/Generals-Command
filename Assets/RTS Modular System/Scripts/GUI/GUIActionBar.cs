using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using RTSModularSystem.Selection;

using Selectable = RTSModularSystem.Selection.Selectable;

namespace RTSModularSystem
{

    public class GUIActionBar : MonoBehaviour
    {
        public GameObject actionButtonPrefab; //the button prefab that will be created for each action
        public ProductionScreen productionScreen;

        private GameObject actionsGridLayout; //the grid layout that holds all the action buttons
        private UIBehaviour[] UIElements; //every ui element on this gameobject

        private bool menuOpen; //whether the menu is open or not
        private PlayerObject currentObject; //the currently selected object
        private PlayerObject previousObject; //the previously selected object

        private List<GUIActionButton> buttons; //the action buttons currently on screen

        //create a button for each build action in the build manager
        public void Init()
        {
            UIElements = GetComponents<UIBehaviour>();
            actionsGridLayout = GetComponentInChildren<GridLayoutGroup>().gameObject;
            buttons = new List<GUIActionButton>();

            //hide menu initially
            currentObject = null;
            previousObject = null;
            ReplaceActions();
        }


        //check if any objects are selected
        void Update()
        {
            //if only one object is selected, display actions
            if (SelectionController.instance.selectedObjects.Count == 1)
            {
                //not a lot of ways to get something out of a hashset
                foreach (Selectable selectable in SelectionController.instance.selectedObjects)
                    currentObject = selectable.GetComponent<PlayerObject>();

                //replace actions if necessary
                ReplaceActions();
                foreach (GUIActionButton button in buttons)
                    button.OnUpdate();
            }
            else
            {
                currentObject = null;
                previousObject = null;
                ToggleMenu(false);
            }
        }


        //removes all children and replaces them with the actions of the new object
        private void ReplaceActions()
        {
            //if current object is null, hide menu
            if (currentObject == null)
            {
                previousObject = null;
                ToggleMenu(false);
                return;
            }

            //if object is same as last frame, no need to change actions
            if (currentObject == previousObject)
                return;
            
            int actionCount = currentObject.data.actions.Count;

            //clear all pre-existing children
            for (int i = actionsGridLayout.transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(actionsGridLayout.transform.GetChild(i).gameObject);

            buttons.Clear();

            //create a button and set its name, sprite and delegate action
            for (int i = 0; i < actionCount; i++)
            {
                //check if this is a action that appears in the bar
                GameActionData actionData = currentObject.GetActionData(i);
                if (!actionData.showOnActionBar)
                    continue;

                GameObject button = Instantiate(actionButtonPrefab, actionsGridLayout.transform);
                Image icon = button.GetComponentInChildren<Image>();
                TextMeshProUGUI textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
                GUIActionButton guiActionButton = button.GetComponent<GUIActionButton>();

                //initialise button to check resource values
                buttons.Add(guiActionButton);
                guiActionButton.Init(actionData.resourceChange);

                //set name and sprite from action data
                icon.sprite = actionData.icon;
                if (actionData.name.Length > 5 && actionData.name.Substring(0,5) == "Spawn")
                    textMesh.text = actionData.name.Substring(6);
                else
                    textMesh.text = actionData.name;

                //the value of i and currentObject will change, create dummy variables for the listener
                int j = i;
                PlayerObject po = currentObject;
                button.GetComponent<Button>().onClick.AddListener(delegate { productionScreen.OpenScreen(po, po.GetActionData(j)); });
            }

            //show menu if this object has an action that appears on the action bar
            ToggleMenu(true);
            previousObject = currentObject;
        }


        //open or close the action menu
        private void ToggleMenu(bool open)
        {
            if (UIElements == null)
                return;

            //show/hide the menu
            foreach (MonoBehaviour mb in UIElements)
                mb.enabled = open;

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(open);
            }
        }
    }
}
