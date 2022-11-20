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

    PlayerObject playerObject;
    GameActionData gameActionData;
    List<ResourceQuantity> currentResources;

    public void OpenScreen(PlayerObject po, GameActionData data)
    {
        playerObject = po;
        gameActionData = data;

        title.text = gameActionData.name;
        description.text = gameActionData.description;
        preview.sprite = gameActionData.icon;

        DisplayCosts();

        produceButton.onClick.AddListener(delegate
        {
            if (!gameActionData.queueAction)
            {
                placeButton.gameObject.SetActive(true);
                closeButton.gameObject.SetActive(true);
                placeButton.onClick.AddListener(delegate
                {
                    RTSPlayer.localPlayer.SetTrigger(playerObject, gameActionData, true);
                    placeButton.onClick.RemoveAllListeners();
                    closeButton.onClick.RemoveAllListeners();
                    placeButton.gameObject.SetActive(false);
                    closeButton.gameObject.SetActive(false);
                });
                closeButton.onClick.AddListener(delegate
                {
                    RTSPlayer.localPlayer.SetTrigger(playerObject, gameActionData, false);
                    placeButton.onClick.RemoveAllListeners();
                    closeButton.onClick.RemoveAllListeners();
                    placeButton.gameObject.SetActive(false);
                    closeButton.gameObject.SetActive(false);
                });
            }

            playerObject.StartAction(playerObject.GetActionIndex(gameActionData));
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


    void Update()
    {
        DisplayCosts();
    }


    void DisplayCosts()
    {
        moneyCost.text = "0";
        scrapCost.text = "0";
        oilCost.text = "0";
        partsCost.text = "0";
        uraniumCost.text = "0";

        moneyCost.color = Color.black;
        scrapCost.color = Color.black;
        oilCost.color = Color.black;
        partsCost.color = Color.black;
        uraniumCost.color = Color.black;

        currentResources = ResourceManager.instance.GetResourceValues(RTSPlayer.GetID(), RTSPlayer.GetID());

        bool canAfford = true;

        foreach (ResourceQuantity cost in CostModifier.GetModifiedCost(gameActionData))
        {
            switch (cost.resourceType)
            {
                case ResourceType.Money:
                    moneyCost.text = (-cost.quantity).ToString();
                    if (currentResources[0].quantity < -cost.quantity)
                    {
                        moneyCost.color = Color.red;
                        canAfford = false;
                    }
                    break;


                case ResourceType.Scrap:
                    scrapCost.text = (-cost.quantity).ToString();
                    if (currentResources[1].quantity < -cost.quantity)
                    {
                        scrapCost.color = Color.red;
                        canAfford = false;
                    }
                    break;


                case ResourceType.Oil:
                    oilCost.text = (-cost.quantity).ToString();
                    if (currentResources[2].quantity < -cost.quantity)
                    {
                        oilCost.color = Color.red;
                        canAfford = false;
                    }
                    break;


                case ResourceType.MechanicalParts:
                    partsCost.text = (-cost.quantity).ToString();
                    if (currentResources[3].quantity < -cost.quantity)
                    {
                        partsCost.color = Color.red;
                        canAfford = false;
                    }
                    break;


                case ResourceType.Uranium:
                    uraniumCost.text = (-cost.quantity).ToString();
                    if (currentResources[4].quantity < -cost.quantity)
                    {
                        uraniumCost.color = Color.red;
                        canAfford = false;
                    }
                    break;
            }
        }

        produceButton.gameObject.SetActive(canAfford);
    }
}
