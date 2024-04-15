using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using WeirdBrothers.ThirdPersonController;
using Unity.Networking.Transport;
using System;
using System.Linq;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance;
    [SerializeField] TMP_Text RedTeamScore, BlueTeamScore,WinnerTitle,TimerLabel,StartTimeLabel,myTeamWinnerPanelScore,AnotherTeamWinnerPanelScore;
    [SerializeField] GameObject WinnerAnnouncementPanel,HostDisconnection;
    [SerializeField] WinnerPanelRow[] MyTeamRow,OtherTeamRows;
    [SerializeField] float TimerClock = 300f;
    [SerializeField] float TimeBeforeStart = 11f;
    const int WinThreshold = 30;
    internal bool GameHasFinished = false;

    int RedScore=0, BlueScore=0;
    private void Awake()
    {
        Instance = this;
        //if(NetworkManager.Singleton.IsServer)
        //{
        //    GetComponent<NetworkObject>().Spawn();
        //}
        //TimeBeforeStart = 3f;
    }

    private void Start()
    {
        LobbyManager.Instance.StopCoroutineofWait();
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleDisconnection;
        RenderSettings.skybox = ItemReference.Instance.colorReference.skyboxes[int.Parse(LobbyManager.Instance.JoinedLobby.Data["Skybox"].Value)];
    }

    private void HandleDisconnection(ulong obj)
    {
        if(obj==NetworkManager.ServerClientId && !GameHasFinished)
        {
            ShowMe();
        }
    }

    [ServerRpc (RequireOwnership =false)]
    public void SetTeamScoreScoreServerRpc(int score, bool isRed = true)
    {
        if (isRed)
            RedScore += score;
        else
            BlueScore += score;


        UpdateTeamScoresClientRpc(RedScore,BlueScore);
        CancelInvoke(nameof(LookForWinner));
        Invoke(nameof(LookForWinner),3f);
    }

    private void LookForWinner()
    {
        if (RedScore>=WinThreshold)
        {
            ShowWinnerClientRpc();
        }
        else if(BlueScore >= WinThreshold)
        {
            ShowWinnerClientRpc(false);
        }
    }

    [ClientRpc]
    private void ShowWinnerClientRpc(bool isRedteamWon=true)
    {
       Debug.LogError("HTTH");
       WinnerAnnouncementPanel.SetActive(true);
        GameHasFinished = true;
        if (isRedteamWon)
        {
            WinnerTitle.text = CustomProperties.Instance.isRed?"VICTORY": "DEFEAT";
        }
       else
        {
            WinnerTitle.text = !CustomProperties.Instance.isRed ? "VICTORY" : "DEFEAT";
        }
        myTeamWinnerPanelScore.text = CustomProperties.Instance.isRed ? RedScore.ToString() : BlueScore.ToString();
        AnotherTeamWinnerPanelScore.text = CustomProperties.Instance.isRed ? BlueScore.ToString() : RedScore.ToString();
        SetPlayerScores();
        WBUIActions.EnableBlackPanel?.Invoke(false);
        AdmobAds.Instance.ShowInterstitialAd();
    }

    private void SetPlayerScores()
    {
        var myteam = GetMyTeamScore();
        var otherTeam = GetAnotherTeamScore();
        for (int i = 0; i < myteam.Count; i++)
        {
            MyTeamRow[i].SetData(new PlayerData(myteam[i].playername.Value.ToString(), myteam[i].kills.Value), myteam[i].IsLocalPlayer);
        }
        for (int i = 0; i < otherTeam.Count; i++)
        {
            OtherTeamRows[i].SetData(new PlayerData(otherTeam[i].playername.Value.ToString(), otherTeam[i].kills.Value), otherTeam[i].IsLocalPlayer);
        }
    }

    private List<PlayerCreator> GetMyTeamScore()
    {
        List<PlayerCreator> players = new();
        foreach (var item in FindObjectsOfType<PlayerCreator>())
        {
            if(item.isRedTeam.Value==CustomProperties.Instance.isRed)
            {
                players.Add(item);
            }
        }
        players = BubbleSort(players);
        return players;
    }
    private List<PlayerCreator> GetAnotherTeamScore()
    {
        List<PlayerCreator> players = new();
        foreach (var item in FindObjectsOfType<PlayerCreator>())
        {
            if(item.isRedTeam.Value!=CustomProperties.Instance.isRed)
            {
                players.Add(item);
            }
        }
        players = BubbleSort(players); //players.OrderByDescending(obj => obj.kills).ToList();
        return players;
    }

    [ClientRpc]
    private void CallDrawClientRpc()
    {
        Debug.LogError("HTTH");
        GameHasFinished = true;
        WinnerAnnouncementPanel.SetActive(true);
        WinnerTitle.text = "DRAW";
        AdmobAds.Instance.ShowInterstitialAd();
    }

    public List<PlayerCreator> BubbleSort(List<PlayerCreator> list)
    {
        int n = list.Count;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (list[j].kills.Value < list[j + 1].kills.Value)
                {
                    PlayerCreator temp = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = temp;
                }
            }
        }
        return list;
    }


    [ClientRpc]
    internal void UpdateTeamScoresClientRpc(int redScore,int blueScore)
    {
        RedScore = redScore;
        BlueScore = blueScore;

        if(CustomProperties.Instance.isRed)
        {
            RedTeamScore.text = blueScore.ToString();
            BlueTeamScore.text = redScore.ToString();
        }
        else
        {
            RedTeamScore.text = redScore.ToString();
            BlueTeamScore.text = blueScore.ToString();
        }
       
    }

    private void FixedUpdate()
    {
        if (GameHasFinished) return;
        if(IsServer)
        {
            if(TimeBeforeStart>0)
            {
                TimeBeforeStart -= Time.deltaTime;
                UpdateStartTime(TimeBeforeStart);
            }
            else if(StartTimeLabel.gameObject.activeSelf)
            {
                StartTimeLabel.gameObject.SetActive(false);
                InvokeRepeating(nameof(CheckTeams), 10f, 5f);
                TimeBeforeStart = 0;
                LobbyManager.Instance.GameHasStarted = true;
                KillStreakSystem.Instance.SetCrates();
                DisableTimerStart();
            }
            if(LobbyManager.Instance.GameHasStarted)
            {
                TimerClock -= Time.deltaTime;
                UpdateTimer(TimerClock);
            }
            
        }

    }

    void CheckTeams()
    {
        var players = FindObjectsOfType<PlayerCreator>();
        int count=0;
        foreach (var item in players)
        {
            if (item.isRedTeam.Value)
                count += 1;
        }
        if(count==0) ShowWinnerClientRpc(false);
        count = 0;
        foreach (var item in players)
        {
            if (!item.isRedTeam.Value)
                count += 1;
        }
        if (count == 0) ShowWinnerClientRpc();
    }

    private void DisableTimerStart()
    {
        DisableStartTimerClientRpc();
        DespawnAndRespawnAll();
    }

    private void DespawnAndRespawnAll()
    {
        var tmps = FindObjectsOfType<WBThirdPersonController>();
        foreach (var item in tmps)
        {
            item.NetworkObject.Despawn(true);
        }
        SpawnAllAgainClientRpc();
    }

   

    public void UpdateTimer(float time)
    {
        if (time < 0) time = 0;
        UpdateTimerClientRpc(time);
        if(time==0)
        {
            if (RedScore < BlueScore)
                ShowWinnerClientRpc(false);
            else
                ShowWinnerClientRpc();
        }
    }

    public void UpdateStartTime(float time)
    {
        if (time < 0) time = 0;
        UpdateStartTimerClientRpc(time);
       
    }

    [ClientRpc]
    public void UpdateTimerClientRpc(float time)
    {
        TimerClock = time;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        TimerLabel.text = minutes.ToString("00")+":" + seconds.ToString("00");
    }
    [ClientRpc]
    public void UpdateStartTimerClientRpc(float time)
    {
        int seconds = Mathf.FloorToInt(time % 60);
        if (!StartTimeLabel.gameObject.activeSelf) StartTimeLabel.gameObject.SetActive(true);
        StartTimeLabel.text = "Game starts in "+seconds.ToString();
    }

    [ClientRpc]
    void DisableStartTimerClientRpc()
    {
        LobbyManager.Instance.GameHasStarted = true;
        StartTimeLabel.gameObject.SetActive(false);
    }

    public void CloseTheGame()
    {
        LobbyManager.Instance.Getout();
    }

    [ClientRpc]
    private void SpawnAllAgainClientRpc()
    {
        PlayerSetManager.instance.ChangeView(40f);
        PlayerSetManager.instance.SpinTheWheel();
    }

    [ServerRpc(RequireOwnership = false)]
    internal void SetKillServerRpc(ulong id)
    {
        foreach (var item in FindObjectsOfType<PlayerCreator>())
        {
            if(item.OwnerClientId==id)
            {
                item.UpdateKillsClientRpc();
            }
        }
        
    }

    internal void ShowMe()
    {
        WinnerAnnouncementPanel.SetActive(true);
        GameHasFinished = true;
        WinnerTitle.text = "VICTORY";
        myTeamWinnerPanelScore.text = CustomProperties.Instance.isRed ? RedScore.ToString() : BlueScore.ToString();
        AnotherTeamWinnerPanelScore.text = CustomProperties.Instance.isRed ? BlueScore.ToString() : RedScore.ToString();
        AnotherTeamWinnerPanelScore.text = "";
        MyTeamRow[0].SetData(new PlayerData(PlayerPrefs.GetString("PlayerName"), CustomProperties.Instance.kills), true);
        WBUIActions.EnableBlackPanel?.Invoke(false);
        AdmobAds.Instance.ShowInterstitialAd();
    }


}

public class PlayerData
{
    public string Name;
    public int Kills;

    public PlayerData(string name,int kills)
    {
        Name = name;
        Kills = kills;
    }
}