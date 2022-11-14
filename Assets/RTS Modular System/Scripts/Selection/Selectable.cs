using UnityEngine;
using UnityEngine.Events;

namespace RTSModularSystem.Selection
{
    //Class that allows an object to be selected in-game
    [RequireComponent(typeof(Outline))]
    public class Selectable : MonoBehaviour
    {
        private Outline outline;

        [Header("Selected Icon")]
        [SerializeField] [Tooltip("The sprite that appears in the UI when selected")]
        public Sprite UIIcon;

        [Header("Outline Settings")]
        [SerializeField] [Tooltip("Whether this selectable uses the properties set on the selection controller")]
        private bool useGlobalOutlineSettings = true;
        [SerializeField] [ConditionalHide("useGlobalOutlineSettings", "false")]
        private float outlineWidth;
        [SerializeField] [ConditionalHide("useGlobalOutlineSettings", "false")]
        private Color outlineColour;

        public UnityEvent OnSelect;
        public UnityEvent OnDeselect;

        //set up selection and outline
        public void Init()
        {
            SelectionController.instance.AddToAvailable(this);

            outline = gameObject.GetComponent<Outline>();
            outline.enabled = false;

            if (useGlobalOutlineSettings)
            {
                outline.OutlineWidth = SelectionController.instance.globalOutlineWidth;
                outline.OutlineColor = SelectionController.instance.globalOutlineColour;
            }
            else
            {
                outline.OutlineWidth = outlineWidth;
                outline.OutlineColor = outlineColour;
            }
        }

        
        //turns on outline when selected
        public void OnSelected()
        {
            outline.enabled = true;
            OnSelect?.Invoke();
            Debug.Log(name + " selected");
        }

        
        //turns off outline when deselected
        public void OnDeselected()
        {
            outline.enabled = false;
            OnDeselect?.Invoke();
            Debug.Log(name + " deselected");
        }


        //remove self from selection controller
        private void OnDestroy()
        {
            Destroy(outline);
            SelectionController.instance.RemoveFromAvailable(this);
            GUISelected.instance.RemoveSelectedIcon(this);
        }
    }
}
