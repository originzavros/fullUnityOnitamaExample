using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    public Button MenuButton;
    public GameObject MenuPanel;

    public Button RestartGameButton;    
    void Start()
    {
        EventManager.StartListening("GameOver", OnGameOver);
    }
    public GameObject gameOverTxtObject;
    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGameOver(string eventName, ActionParams data)
    {
        EnableGameOverText();
    }

    public void EnableGameOverText()
    {
        gameOverTxtObject.SetActive(true);
    }
    public void DisableGameOverText()
    {
        gameOverTxtObject.SetActive(false);
    }

    public void OnClickMenu()
    {
        if(MenuPanel.activeSelf == false)
        {
            MenuPanel.SetActive(true);
        }
        else{
            MenuPanel.SetActive(false);
        }
    }

    public void OnClickRestartButton()
    {
        OnClickMenu();
        DisableGameOverText();
        GameManager.RestartGame();
    }

    public void OnClickMainMenuButton()
    {
        SceneManager.LoadScene("StartMenu");
    }


}
