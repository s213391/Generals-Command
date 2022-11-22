using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTSModularSystem.GameResources;
using RTSModularSystem;
using TMPro;

public class GUIActionButton : MonoBehaviour
{
    public Button button;
    public Image icon;
    public TextMeshProUGUI textMesh;

    public void Init(PlayerObject po, GameActionData data, ProductionScreen productionScreen)
    {
        icon.sprite = data.icon;
        if (data.name.Length > 5 && data.name.Substring(0, 5) == "Spawn")
            textMesh.text = data.name.Substring(6);
        else
            textMesh.text = data.name;

        button.onClick.AddListener(delegate { productionScreen.OpenScreen(po, data); });
    }
}
