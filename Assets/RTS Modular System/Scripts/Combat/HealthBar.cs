using UnityEngine;
using UnityEngine.UI;

namespace DS_BasicCombat
{
    //creates a health bar that follows the object it is attached to
    public class HealthBar : MonoBehaviour
    {

        public Attackable attackable;
        public Image image;
        private Image background;
        private RectTransform rectTransform;

        [SerializeField]
        private bool hideWhenFullHealth;
        private float heightOffset;

        //set up health bar
        public void Init(float objectHeight, float objectWidth)
        {
            background = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
            if (image)
            {
                image.type = Image.Type.Filled;
                image.fillMethod = Image.FillMethod.Horizontal;
            }

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x * objectWidth, rectTransform.sizeDelta.y);

            //set the height of the health bar based on the object height
            heightOffset = objectHeight * 1.7f;
        }


        //update position
        public void OnUpdate()
        {
            transform.position = attackable.transform.position + Vector3.up * heightOffset;
            transform.forward = Camera.main.transform.forward;
            UpdateMeter();
        }


        //update the amount of health bar visible
        public void UpdateMeter()
        {
            //hide the health bar under certain conditions
            if (!attackable.isVisible || (hideWhenFullHealth && attackable.currentHealth == attackable.maxHealth))
            {
                image.enabled = false;
                background.enabled = false;
            }
            else
            {
                image.enabled = true;
                background.enabled = true;

                // scale the meter
                float pct = Mathf.Clamp01((float)attackable.currentHealth / (float)attackable.maxHealth);
                image.fillAmount = pct;
            }
        }
    }
}
