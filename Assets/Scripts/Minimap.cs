using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTSModularSystem;

public class Minimap : MonoBehaviour
{
    public static Minimap instance { get; private set; }

    public GameObject renderTexture;
    public GameObject iconPrefab;
    public int iconSize = 10;
    
    List<PlayerObject> mapIcons = new List<PlayerObject>();

    Vector2 mapBottomCorner;
    RectTransform rectTrans;
    
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
        {
            instance = this;

            rectTrans = GetComponent<RectTransform>();
            mapBottomCorner = rectTrans.anchorMin;
        }
    }

    //update the positions of icons on the map
    void LateUpdate()
    {
        for (int i = 0; i < mapIcons.Count; i++)
        {
            Vector2 mapIconsPos = new Vector2(mapIcons[i].transform.position.x - 350.0f, mapIcons[i].transform.position.z - 400.0f);
            renderTexture.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = mapIconsPos;
        }
    }


    public void RegisterIcon(PlayerObject po, Sprite icon, Color colour)
    {
        mapIcons.Add(po);

        GameObject newIcon = Instantiate(iconPrefab, renderTexture.transform);
        newIcon.GetComponent<RectTransform>().sizeDelta = Vector2.one * iconSize;
        newIcon.GetComponent<Image>().sprite = icon;
        newIcon.GetComponent<Image>().color = colour;
    }


    public void UnRegisterIcon(PlayerObject po)
    {
        if (mapIcons.Contains(po))
        {
            int index = mapIcons.IndexOf(po);
            mapIcons.RemoveAt(index);
            Destroy(renderTexture.transform.GetChild(index));
        }
    }
}
