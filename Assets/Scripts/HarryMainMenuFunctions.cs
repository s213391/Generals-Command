using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarryMainMenuFunctions : MonoBehaviour
{
    //Harry made this, yes I am stupid :)
    public GameObject MainMenuButtons;
    public GameObject HostGameButtons;
    public GameObject JoinGameButtons;
    public GameObject OptionsButtons;
    public GameObject CreditsButtons;
    public GameObject QuitApplicationButtons;

    public bool HostGameMenuOpen = false;
    public bool JoinGameMenuOpen = false;
    public bool OptionsMenuOpen = false;
    public bool CreditsOpen = false;
    public bool QuitGame = false;

    public bool menuClosed = false;

    public Button HostGameButton;
    public Button JoinGameButton;
    public Button OptionsMenuButton;
    public Button CreditsButton;
    public Button QuitGameButton;

    public Button HostGameBackButton;
    public Button JoinGameBackButton;
    public Button OptionsBackButton;
    public Button CreditsBackButton;
    public Button QuitGameBackButton;

    public Button QuitApplicationButton;

    // FOR ANIMATION WAIT TIME
    public bool animationStillGoing = true;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuButtons.SetActive(false);

        StartCoroutine(MainMenuWait());

        // start buttons
        Button btn1 = HostGameButton.GetComponent<Button>();
        btn1.onClick.AddListener(OpenTheHostGameMenu);

        Button btn2 = JoinGameButton.GetComponent<Button>();
        btn2.onClick.AddListener(OpenJoinGameMenu);

        Button btn3 = OptionsMenuButton.GetComponent<Button>();
        btn3.onClick.AddListener(OpenOptionsMenu);

        Button btn4 = CreditsButton.GetComponent<Button>();
        btn4.onClick.AddListener(OpenCredits);

        Button btn5 = QuitGameButton.GetComponent<Button>();
        btn5.onClick.AddListener(OpenQuitGameMenu);

        // back buttons
        Button btn6 = HostGameBackButton.GetComponent<Button>();
        btn6.onClick.AddListener(HostGameBack);

        Button btn7 = JoinGameBackButton.GetComponent<Button>();
        btn7.onClick.AddListener(JoinGameMenuBack);

        Button btn8 = OptionsBackButton.GetComponent<Button>();
        btn8.onClick.AddListener(OptionsBack);

        Button btn9 = CreditsBackButton.GetComponent<Button>();
        btn9.onClick.AddListener(CreditsBack);

        Button btn10 = QuitGameBackButton.GetComponent<Button>();
        btn10.onClick.AddListener(QuitBack);

        //Quit Applicationn Button

        Button btn11 = QuitApplicationButton.GetComponent<Button>();
        btn11.onClick.AddListener(QuitApplication);
    }

    // Update is called once per frame
    void Update()
    {
        if (HostGameMenuOpen)
        {
            MainMenuButtons.SetActive(false);
            HostGameButtons.SetActive(true);
        } else 
        {
            if (animationStillGoing == false && menuClosed == false)
            {
                MainMenuButtons.SetActive(true);
                HostGameButtons.SetActive(false);
            }
        }
        
        if (JoinGameMenuOpen)
        {
            MainMenuButtons.SetActive(false);
            JoinGameButtons.SetActive(true);
        }
        else 
        {
            if (animationStillGoing == false && menuClosed == false)
            {
                MainMenuButtons.SetActive(true);
                JoinGameButtons.SetActive(false);
            }
        }

        if (OptionsMenuOpen)
        {
            MainMenuButtons.SetActive(false);
            OptionsButtons.SetActive(true);
        } else 
        {
            if (animationStillGoing == false && menuClosed == false)
            {
                MainMenuButtons.SetActive(true);
                OptionsButtons.SetActive(false);
            }
        }

        if (CreditsOpen)
        {
            MainMenuButtons.SetActive(false);
            CreditsButtons.SetActive(true);
        }
        else
        {
            if (animationStillGoing == false && menuClosed == false)
            {
                MainMenuButtons.SetActive(true);
                CreditsButtons.SetActive(false);
            }
        }

        if (QuitGame)
        {
            MainMenuButtons.SetActive(false);
            QuitApplicationButtons.SetActive(true);
        } else 
        {
            if (animationStillGoing == false && menuClosed == false)
            {
                MainMenuButtons.SetActive(true);
                QuitApplicationButtons.SetActive(false);
            }
        }
    }
    // start button void
    void OpenTheHostGameMenu()
    {
        HostGameMenuOpen = true;
        menuClosed = true;
    }

    void OpenJoinGameMenu()
    {
        JoinGameMenuOpen = true;
        menuClosed = true;
    }

    void OpenOptionsMenu()
    {
        OptionsMenuOpen = true;
        menuClosed = true;
    }

    void OpenCredits()
    {
        CreditsOpen = true;
        menuClosed = true;
    }

    void OpenQuitGameMenu()
    {
        QuitGame = true;
        menuClosed = true;
    }
    
    // Back buttons
    void HostGameBack()
    {
        HostGameMenuOpen = false;
        menuClosed = false;
    }

    void JoinGameMenuBack()
    {
        JoinGameMenuOpen = false;
        menuClosed = false;
    }

    void OptionsBack()
    {
        OptionsMenuOpen = false;
        menuClosed = false;
    }

    void CreditsBack()
    {
        CreditsOpen = false;
        menuClosed = false;
    }

    void QuitBack()
    {
        QuitGame = false;
        menuClosed = false;
    }

    void QuitApplication()
    {
        Application.Quit();
    }

    IEnumerator MainMenuWait()
    {
        yield return new WaitForSeconds(3.1f);
        animationStillGoing = false;
        MainMenuButtons.gameObject.SetActive(true);
    }
}
