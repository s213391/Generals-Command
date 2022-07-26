using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUIPlayerScore : MonoBehaviour
{
    public static GUIPlayerScore instance;
    
    public List<GameObject> playerScores;
    public List<int> playerObjectHealths;
    public float flashDuration = 0.2f;
    public int commandCentreHealth = 2000;

    private List<TextMeshProUGUI> playerNames = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> playerHealth = new List<TextMeshProUGUI>();
    private List<bool> playerHealthFlashing = new List<bool>();

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;

        playerObjectHealths = new List<int>();
        
        for (int i = 0; i < GameData.instance.playerData.Count; i++)
        {
            playerScores[i].SetActive(true);
            
            playerNames.Add(playerScores[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>());
            playerHealth.Add(playerScores[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>());

            playerNames[i].text = GameData.instance.playerData[i].name;
            playerHealth[i].text = "100%";
            playerNames[i].color = GameData.instance.playerData[i].colour;
            playerHealth[i].color = GameData.instance.playerData[i].colour;

            playerObjectHealths.Add(commandCentreHealth);
            playerHealthFlashing.Add(false);
        }

        for (int i = GameData.instance.playerData.Count; i < playerScores.Count;i++)
            playerScores[i].SetActive(false);
    }


    //updates health values for the given player
    public void UpdateHealth(int playerNumber, int newHealth)
    {
        int percent = 100 * newHealth / commandCentreHealth;
        playerHealth[playerNumber].text = percent.ToString() + "%";
        playerObjectHealths[playerNumber] = newHealth;

        if (!playerHealthFlashing[playerNumber])
            StartCoroutine(FlashHealthWhite(playerNumber));
    }


    //turns a player's name white then back for a set time to indicate damage
    private IEnumerator FlashHealthWhite(int playerNumber)
    {
        playerHealthFlashing[playerNumber] = true;

        playerHealth[playerNumber].color = Color.white;
        yield return new WaitForSeconds(flashDuration);

        playerHealth[playerNumber].color = GameData.instance.playerData[playerNumber].colour;
        yield return new WaitForSeconds(flashDuration);

        playerHealthFlashing[playerNumber] = false;
    }
}
