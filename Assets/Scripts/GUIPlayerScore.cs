using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUIPlayerScore : MonoBehaviour
{
    public static GUIPlayerScore instance;
    
    public List<GameObject> playerScores;

    private List<TextMeshProUGUI> playerNames;
    private List<TextMeshProUGUI> playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
        
        for (int i = 0; i < playerScores.Count; i++)
        {
            playerNames.Add(playerScores[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>());
            playerHealth.Add(playerScores[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>());

            playerNames[i].text = GameData.instance.playerData[i].name;
            playerHealth[i].text = "100%";
            playerNames[i].color = GameData.instance.playerData[i].colour;
            playerHealth[i].color = GameData.instance.playerData[i].colour;
        }
    }

    //updates health values for the given player
    public void UpdateHealth(int playerNumber, int newHealth)
    {
        playerHealth[playerNumber].text = newHealth.ToString() + "%";
    }
}
