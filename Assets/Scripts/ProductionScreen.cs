using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTSModularSystem;
using RTSModularSystem.GameResources;
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
    public Button placeButton;
    public Button closeButton;
    
    public void OpenScreen(PlayerObject po, GameActionData data)
    {
        title.text = data.name;
        description.text = data.description;
        preview.sprite = data.icon;

        moneyCost.text = "0";
        scrapCost.text = "0";
        oilCost.text = "0";
        partsCost.text = "0";
        uraniumCost.text = "0";

        foreach (ResourceQuantity cost in CostModifier.GetModifiedCost(data))
        {
            switch (cost.resourceType)
            {
                case ResourceType.Money:
                    moneyCost.text = cost.quantity.ToString();
                    if (moneyCost.text.StartsWith("-"))
                        moneyCost.text = moneyCost.text.Substring(1);
                    break;
                case ResourceType.Scrap:
                    scrapCost.text = cost.quantity.ToString();
                    if (scrapCost.text.StartsWith("-"))
                        scrapCost.text = scrapCost.text.Substring(1);
                    break;
                case ResourceType.Oil:
                    oilCost.text = cost.quantity.ToString();
                    if (oilCost.text.StartsWith("-"))
                        oilCost.text = oilCost.text.Substring(1);
                    break;
                case ResourceType.MechanicalParts:
                    partsCost.text = cost.quantity.ToString();
                    if (partsCost.text.StartsWith("-"))
                        partsCost.text = partsCost.text.Substring(1);
                    break;
                case ResourceType.Uranium:
                    uraniumCost.text = cost.quantity.ToString();
                    if (uraniumCost.text.StartsWith("-"))
                        uraniumCost.text = uraniumCost.text.Substring(1);
                    break;
            }
        }

        produceButton.onClick.AddListener(delegate { 
            if (!data.queueAction)
            {
                placeButton.gameObject.SetActive(true);
                closeButton.gameObject.SetActive(true);
                placeButton.onClick.AddListener(delegate
                {
                    RTSPlayer.localPlayer.SetTrigger(po, data, true);
                    placeButton.onClick.RemoveAllListeners();
                    closeButton.onClick.RemoveAllListeners();
                    placeButton.gameObject.SetActive(false);
                    closeButton.gameObject.SetActive(false);
                });
                closeButton.onClick.AddListener(delegate
                {
                    RTSPlayer.localPlayer.SetTrigger(po, data, false);
                    placeButton.onClick.RemoveAllListeners();
                    closeButton.onClick.RemoveAllListeners();
                    placeButton.gameObject.SetActive(false);
                    closeButton.gameObject.SetActive(false);
                });
            }

            po.StartAction(po.GetActionIndex(data));
            CloseScreen();
        });
        cancelButton.onClick.AddListener(CloseScreen);

        gameObject.SetActive(true);
    }


    public void CloseScreen()
    {
        title.text = "No Title";
        description.text = "No Description";
        preview.sprite = null;

        moneyCost.text = "0";
        scrapCost.text = "0";
        oilCost.text = "0";
        partsCost.text = "0";
        uraniumCost.text = "0";

        produceButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        gameObject.SetActive(false);
    }
}
