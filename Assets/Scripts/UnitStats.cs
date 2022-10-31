using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RTSModularSystem;
using DS_Selection;
using DS_BasicCombat;

using Selectable = DS_Selection.Selectable;

public class UnitStats : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public RawImage damageImage;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI resistance1Text;
    public TextMeshProUGUI resistance2Text;
    public TextMeshProUGUI resistance3Text;
    public TextMeshProUGUI resistance4Text;

    PlayerObject currentObject;

    public List<Texture> damageIcons = new List<Texture>();

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;

        foreach (Selectable selectable in SelectionController.instance.selectedObjects)
        {
            if (currentObject == selectable)
                return;
            else
                currentObject = selectable.GetComponent<PlayerObject>();
        }

        if (currentObject == null)
            return;

        Attackable attackable = currentObject.GetComponent<Attackable>();
        if (attackable == null)
        {
            healthText.text = "N/A";
            resistance1Text.text = "N/A";
            resistance2Text.text = "N/A";
            resistance3Text.text = "N/A";
            resistance4Text.text = "N/A";
        }
        else
        {
            healthText.text = attackable.currentHealth.ToString() + "/" + attackable.maxHealth.ToString();

            List<int> resistances = attackable.GetResistances();
            resistance1Text.text = resistances[0].ToString() + "%";
            resistance2Text.text = resistances[1].ToString() + "%";
            resistance3Text.text = resistances[2].ToString() + "%";
            resistance4Text.text = resistances[3].ToString() + "%";
        }

        Attacker attacker = currentObject.GetComponent<Attacker>();
        if (attacker == null)
        {
            damageText.enabled = false;
            damageImage.enabled = false;
        }
        else
        {
            damageText.enabled = true;
            damageText.text = attacker.attackDamage.ToString();

            damageImage.enabled = true;
            damageImage.texture = damageIcons[(int)attacker.damageType];
        }
    }
}
