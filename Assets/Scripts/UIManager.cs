using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] GameObject MainPanel, CreateRoomPanel, JoinRoomPanel, SettingPanel,SelectionPanel,LevelSelection,leaderBoardPopup;
    [SerializeField] SelectionManager selectionManager;
    [SerializeField] Image ProfileImage;
    [SerializeField] Sprite[] babyImages;
    public TMP_Text LobbyName,PlayerNameLabel;
    [SerializeField] AudioSource Music;
    [SerializeField] Slider Aim;
    [SerializeField] Toggle MusicToggle, SFXToggle;

    public GameObject Blocker;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("PlayerName"))
        {
            PlayerPrefs.SetString("PlayerName", "Guest_" + GenerateRandomNumberString(8));
            PlayerPrefs.SetInt("BabyImage", Random.Range(0, 4));
        }
        PlayerNameLabel.text= PlayerPrefs.GetString("PlayerName");
        if (UnityAuth.Instance.profileImage != null)
            ProfileImage.sprite = UnityAuth.Instance.profileImage;
        else
            ProfileImage.sprite = babyImages[PlayerPrefs.GetInt("BabyImage")];
        //AdmobAds.Instance.LoadBannerAd();
        AdmobAds.Instance.LoadInterstitialAd();
        Aim.value = PlayerPrefs.GetFloat("Aim", .75f);
        if (PlayerPrefs.GetInt("Music", 1) == 1) Music.Play();
        Aim.onValueChanged.AddListener((float val) => { ChangeAim(val); });
        MusicToggle.isOn = PlayerPrefs.GetInt("Music", 1) == 1;
        SFXToggle.isOn = PlayerPrefs.GetInt("SFX", 1) == 1;
        MusicToggle.onValueChanged.AddListener((bool val) => { ChangeMusic(val); });
        SFXToggle.onValueChanged.AddListener((bool val) => { ChangeSFX(val); });
    }

    

    private void ChangeMusic(bool val)
    {
        PlayerPrefs.SetInt("Music", val ? 1 : 0);
        if (PlayerPrefs.GetInt("Music", 1) == 1) Music.Play();
        else Music.Stop();
    }
    private void ChangeSFX(bool val)
    {
        PlayerPrefs.SetInt("SFX", val ? 1 : 0);
    }

    private void ChangeAim(float val)
    {
        PlayerPrefs.SetFloat("Aim", val);
    }

    string GenerateRandomNumberString(int length)
    {
        string result = "";
        System.Random random = new System.Random();

        for (int i = 0; i < length; i++)
        {
            result += random.Next(0, 10).ToString(); // Generating random digits (0-9)
        }

        return result;
    }


    public void quitgame()
    {
        Application.Quit();
    }

    public void CreateGame()
    {
        LobbyManager.Instance.CreatePublicLobby();
    }

    public void JoinGame(string levelName)
    {
        PlayerPrefs.SetString("Level", levelName);
        LobbyManager.Instance.JoinLobby();
        LevelSelection.SetActive(false);
    }

    public void OpenLevelSelection()
    {
        LevelSelection.SetActive(true);
    }

    public void ShowInterstialAd()
    {
        AdmobAds.Instance.ShowInterstitialAd();
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

    public void SetMessage(string message)
    {
        Blocker.SetActive(true);
        LobbyName.text = message;
    }

    public void CloseMessage()
    {
        Blocker.SetActive(false);
    }

    public void LeadShop()
    {
        AdmobAds.Instance.ShowInterstitialAd(loadShop);
        
    }
    

    void loadShop()
    {
        SceneManager.LoadSceneAsync("Shop");
    }  
    
    public void loadLeaderboard()
    {
        if(PlayerPrefs.GetInt("FirstTimeLeaderBoard",0)==0)
        {
            PlayerPrefs.SetInt("FirstTimeLeaderBoard", 1);
            leaderBoardPopup.SetActive(true);
            return;
        }
        UnityAuth.Instance.ShowLeaderboard();
    }
}
