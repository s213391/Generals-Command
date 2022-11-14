using System.Collections.Generic;
using UnityEngine;

namespace RTSModularSystem.Selection
{
    //Class that handles the selection and deselection of objects with a Selectable component
    [DisallowMultipleComponent]
    public class SelectionController : MonoBehaviour
    {
        public static SelectionController instance;
        
        public HashSet<Selectable> selectedObjects { get; private set; } //objects that are currently selected
        public List<Selectable> availableObjects { get; private set; } //objects that are available to be selected
        public List<HashSet<Selectable>> quickSelectGroups { get; private set; } //numbered groups for quick selection of multiple objects

        public float globalOutlineWidth;
        public Color globalOutlineColour;
        public int quickSelectGroupsCount;


        //set up singleton
        void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
            {
                instance = this;
                selectedObjects = new HashSet<Selectable>();
                availableObjects = new List<Selectable>();
                quickSelectGroups = new List<HashSet<Selectable>>();
                for (int i = 0; i < quickSelectGroupsCount; i++)
                    quickSelectGroups.Add(new HashSet<Selectable>());
            }    
        }


        //handles the selection and UI of a selectable
        public void Select(Selectable selectable)
        {
            if (selectedObjects.Add(selectable))
            {
                selectable.OnSelected();
                GUISelected.instance.AddSelectedIcon(selectable);
            }
        }


        //handles the deselection and UI of a selectable
        public void Deselect(Selectable selectable)
        {
            if (selectedObjects.Remove(selectable))
            {
                selectable.OnDeselected();
                GUISelected.instance.RemoveSelectedIcon(selectable);
            }
        }


        //clears all selected objects and the UI
        public void DeselectAll()
        {
            foreach (Selectable selectable in selectedObjects)
                selectable.OnDeselected();

            GUISelected.instance.ClearSelectedIcons();
            selectedObjects.Clear();
        }


        //returns if the selectable is currently selected
        public bool IsSelected(Selectable selectable)
        {
            return selectedObjects.Contains(selectable);
        }


        //adds a selectable to the controller
        public void AddToAvailable(Selectable selectable)
        {
            availableObjects.Add(selectable);
        }


        //removes a selectable from the controller
        public void RemoveFromAvailable(Selectable selectable)
        {
            availableObjects.Remove(selectable);
            selectedObjects.Remove(selectable);
        }

        #region quickSelectGroups

        //adds multiple objects to a quick select group
        public void AddToQuickSelectGroup(List<Selectable> selectables, int groupNumber)
        {
            foreach (Selectable selectable in selectables)
                quickSelectGroups[groupNumber].Add(selectable);
        }


        //adds all selected units to a quick select group
        public void AddSelectedToGroup(int groupNumber)
        {
            foreach (Selectable selectable in selectedObjects)
                quickSelectGroups[groupNumber].Add(selectable);
        }


        //removes multiple objects from a quick select group
        public void RemoveFromQuickSelectGroup(List<Selectable> selectables, int groupNumber)
        {
            foreach (Selectable selectable in selectables)
                quickSelectGroups[groupNumber].Remove(selectable);
        }


        //empties a quick select group
        public void EmptyQuickSelectGroup(int groupNumber)
        {
            quickSelectGroups[groupNumber].Clear();
        }


        //selects everything in a quick select group
        public void SelectGroup(int groupNumber, bool deselectAllCurrent = true)
        {
            if (deselectAllCurrent)
                DeselectAll();

            foreach (Selectable selectable in quickSelectGroups[groupNumber])
                Select(selectable);
        }

        #endregion
    }
}
