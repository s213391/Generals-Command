using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RTSModularSystem.GameResources;

namespace RTSModularSystem
{

    public class GUIResources : MonoBehaviour
    {
        public GameObject resourceCount; //the prefab that shows resource icon and count for one resource

        private GameObject resourceBar; //the bar along the top of the screen that displays the player's resource counts


        //add a resource count to the gui for each resource
        public void Init()
        {
            resourceBar = gameObject;
            List<ResourceData> resourceTypes = ResourceManager.instance.resources;

            //clear all pre-existing children
            for (int i = resourceBar.transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(resourceBar.transform.GetChild(i).gameObject);

            //create each gui resource count and set it to initial values
            List<ResourceQuantity> initialResources = ResourceManager.instance.initialResources;
            //both this and the resource manager create their lists from the object data manager, so the indices will match
            for (int i = 0; i < resourceTypes.Count; i++)
            {
                GameObject newCount = Instantiate(resourceCount, transform);
                Image icon = newCount.GetComponentInChildren<Image>();
                TextMeshProUGUI count = newCount.GetComponentInChildren<TextMeshProUGUI>();

                icon.sprite = resourceTypes[i].guiSprite;
                int index = initialResources.FindIndex(x => x.resourceType == resourceTypes[i].resourceType);
                if (index == -1)
                    count.text = "0";
                else
                    count.text = initialResources[index].quantity.ToString();
            }
        }


        //update the resource count from the local master dictionary
        public void OnUpdate()
        {
            List<ResourceQuantity> currentResources = RTSPlayer.GetResourcesCount();
            if (currentResources != null)
            {
                for (int i = 0; i < currentResources.Count; i++)
                {
                    TextMeshProUGUI count = transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
                    count.text = currentResources[i].quantity.ToString();
                }
            }
            else
                Debug.Log("GUI could not read resources");
        }
    }
}
