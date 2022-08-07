using UnityEngine;
using UnityEngine.UI;

namespace DS_Selection
{
    //Class used in the UI to display the icon of a selected object
    public class SelectedIcon : MonoBehaviour
    {
        public Selectable attachedSelectable;

        public void Init(Selectable selectable)
        {
            attachedSelectable = selectable;
            if (attachedSelectable.UIIcon != null)
                GetComponent<Image>().sprite = attachedSelectable.UIIcon;
        }
    }
}
