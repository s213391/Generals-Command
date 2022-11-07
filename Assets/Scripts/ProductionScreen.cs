using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTSModularSystem;
using TMPro;

public class ProductionScreen : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Image preview;
    public TextMeshProUGUI moneyCost;
    public TextMeshProUGUI scrapCost;
    public TextMeshProUGUI oilCost;
    public TextMeshProUGUI partsCost;
    public TextMeshProUGUI uraniumCost;
    public Button produceButton;
    public Button cancelButton;
    
    public void OpenScreen(PlayerObject po, GameActionData data)
    {

    }


    public void CloseScreen()
    {

    }
}
