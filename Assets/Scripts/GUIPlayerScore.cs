using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUIPlayerScore : MonoBehaviour
{
    public static GUIPlayerScore instance;
    
    public List<GameObject> playerScores;
    public List<int> playerObjectHealths;

    private RectTransform rectTransform;
    private List<TextMeshProUGUI> playerNames = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> playerHealth = new List<TextMeshProUGUI>();

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;

        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 10f + playerScores.Count * 20f);
        playerObjectHealths = new List<int>();

        for (int i = 0; i < 4; i++)
            playerScores[i].SetActive(false);
        
        for (int i = 0; i < GameData.instance.playerData.Count; i++)
        {
            playerScores[i].SetActive(true);
            
            playerNames.Add(playerScores[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>());
            playerHealth.Add(playerScores[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>());

            playerNames[i].text = GameData.instance.playerData[i].name;
            playerHealth[i].text = "100%";
            playerNames[i].color = GameData.instance.playerData[i].colour;
            playerHealth[i].color = GameData.instance.playerData[i].colour;

            playerObjectHealths.Add(1000);
        }
    }

    //updates health values for the given player
    public void UpdateHealth(int playerNumber, int newHealth)
    {
        playerHealth[playerNumber].text = newHealth.ToString() + "%";
        playerObjectHealths[playerNumber] = newHealth;
    }
}
