using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] GameObject MainPanel, CreateRoomPanel, JoinRoomPanel, SettingPanel,SelectionPanel;
    [SerializeField] SelectionManager selectionManager;
    public TMP_Text LobbyName;

    public GameObject Blocker;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void quitgame()
    {
        Application.Quit();
    }

    public void CreateGame()
    {
        LobbyManager.Instance.CreatePublicLobby();
    }

    public void JoinGame()
    {
        LobbyManager.Instance.JoinLobby();
    }

    public void OpenPanel(int i)
    {
        MainPanel.SetActive(true);
        CreateRoomPanel.SetActive(false);
        JoinRoomPanel.SetActive(false);
        SettingPanel.SetActive(false);
        SelectionPanel.SetActive(false);
        selectionManager.DestroyinShow();
        switch(i)
        {
            case 1:
                CreateRoomPanel.SetActive(true);
                break;
            case 2:
                JoinRoomPanel.SetActive(true);
                break;
            case 3:
                SettingPanel.SetActive(true);
                break;
            case 4:
                SelectionPanel.SetActive(true);
                selectionManager.ShowCharacterPanel();
                break;
        }
    }
    
    public void LoadGameAsync(int i)
    {
        //NetWorkController.Instance.SetIsHostAndLoad(i);
    }

    
}
