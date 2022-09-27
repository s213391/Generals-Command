using System.Collections.Generic;
using UnityEngine;

namespace DS_Selection
{
    //Class that handles the selection and deselection of objects with a Selectable component
    [DisallowMultipleComponent]
    public class SelectionController : MonoBehaviour
    {
        public static SelectionController instance;
        
        public HashSet<Selectable> selectedObjects { get; private set; } //objects that are currently selected
        public List<Selectable> availableObjects { get; private set; } //objects that are available to be selected

        public float globalOutlineWidth;
        public Color globalOutlineColour;


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
    }
}
