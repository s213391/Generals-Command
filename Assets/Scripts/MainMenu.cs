using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuScreen;
    public GameObject hostGameScreen;
    public GameObject joinGameScreen;
    public GameObject optionsMenuScreen;
    public GameObject creditsScreen;
    public GameObject quitGameScreen;


    //start showing logo animation
    void Start()
    {
        CloseMenus();
        StartCoroutine(MainMenuWait());
    }


    //closes all menus
    void CloseMenus()
    {
        mainMenuScreen.SetActive(false);
        hostGameScreen.SetActive(false);
        joinGameScreen.SetActive(false);
        optionsMenuScreen.SetActive(false);
        creditsScreen.SetActive(false);
        quitGameScreen.SetActive(false);
    }


    //displays main menu screen
    public void OpenMainMenu()
    { 
        CloseMenus();
        mainMenuScreen.SetActive(true);
    }


    //displays host game screen
    public void OpenHostGameMenu()
    {
        mainMenuScreen.SetActive(false);
        hostGameScreen.SetActive(true);
    }


    //displays join game screen
    public void OpenJoinGameMenu()
    {
        mainMenuScreen.SetActive(false);
        joinGameScreen.SetActive(true);
    }


    //displays options menu screen
    public void OpenOptionsMenu()
    {
        mainMenuScreen.SetActive(false);
        optionsMenuScreen.GetComponent<OptionsMenu>().Open();
        optionsMenuScreen.SetActive(true);
    }


    //displays credits screen
    public void OpenCreditsMenu()
    {
        mainMenuScreen.SetActive(false);
        creditsScreen.SetActive(true);
    }


    //displays quit game screen
    public void OpenQuitGameMenu()
    {
        mainMenuScreen.SetActive(false);
        quitGameScreen.SetActive(true);
    }


    //quits the game
    public void QuitApplication()
    {
        Application.Quit();
    }


    //switch to main menu after logos
    IEnumerator MainMenuWait()
    {
        yield return new WaitForSeconds(3.0f);
        OpenMainMenu();
    }
}
