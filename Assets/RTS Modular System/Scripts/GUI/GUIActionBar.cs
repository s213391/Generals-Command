using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DS_Selection;

using Selectable = DS_Selection.Selectable;

namespace RTSModularSystem
{

    public class GUIActionBar : MonoBehaviour
    {
        public GameObject actionButtonPrefab; //the button prefab that will be created for each action

        private GameObject actionsGridLayout; //the grid layout that holds all the action buttons
        private Image panel; //this gameobject's image component

        private bool menuOpen; //whether the menu is open or not
        private PlayerObject currentObject; //the currently selected object
        private PlayerObject previousObject; //the previously selected object

        //create a button for each build action in the build manager
        public void Init()
        {
            panel = GetComponent<Image>();
            actionsGridLayout = GetComponentInChildren<GridLayoutGroup>().gameObject;

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
            
            //if theres no actions, don't open the menu
            int actionCount = currentObject.data.actions.Count;
            if (actionCount == 0)
            {
                previousObject = currentObject;
                ToggleMenu(false);
                return;
            }

            //clear all pre-existing children
            for (int i = actionsGridLayout.transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(actionsGridLayout.transform.GetChild(i).gameObject);

            //create a button and set its name, sprite and delegate action
            bool hasVisibleAction = false;
            for (int i = 0; i < actionCount; i++)
            {
                //check if this is a action that appears in the bar
                GameActionData actionData = currentObject.GetActionData(i);
                if (!actionData.showOnActionBar)
                    continue;

                GameObject button = Instantiate(actionButtonPrefab, actionsGridLayout.transform);
                Image icon = button.GetComponentInChildren<Image>();
                TextMeshProUGUI textMesh = button.GetComponentInChildren<TextMeshProUGUI>();

                //set name and sprite from action data
                icon.sprite = actionData.icon;
                textMesh.text = actionData.name;

                //the value of i and currentObject will change, create dummy variables for the listener
                int j = i;
                PlayerObject po = currentObject;
                button.GetComponent<Button>().onClick.AddListener(delegate { po.StartAction(j); });

                hasVisibleAction = true;
            }

            //show menu if this object has an action that appears on the action bar
            ToggleMenu(hasVisibleAction);
            previousObject = currentObject;
        }


        //open or close the action menu
        private void ToggleMenu(bool open)
        {
            //show/hide the menu
            if (panel)
                panel.enabled = open;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(open);
            }
        }
    }
}
