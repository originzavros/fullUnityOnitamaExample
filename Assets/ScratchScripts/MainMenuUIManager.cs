using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    public Button SingleplayerButton;
    public Button MultiplayerButton;
    public Button ExitGameButton;
    public Button EasyButton;
    public Button MediumButton;
    public Button HardButton;

    public Button BackButton;

    public GameObject MainPanel;
    public GameObject SingleplayerPanel;
    


    public void OnSinglePlayerButtonClicked()
    {
        MainPanel.SetActive(false);
        SingleplayerPanel.SetActive(true);
    }

    public void OnExitGameClicked()
    {
        Application.Quit();
        Debug.Log("Exit Button Clicked");
    }

    public void OnEasyButtonClicked()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void OnBackButtonClicked()
    {
        MainPanel.SetActive(true);
        SingleplayerPanel.SetActive(false);
    }
}
