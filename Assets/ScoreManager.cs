using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using WeirdBrothers.ThirdPersonController;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance;
    [SerializeField] TMP_Text RedTeamScore, BlueTeamScore,WinnerTitle,TimerLabel,StartTimeLabel;
    [SerializeField] GameObject WinnerAnnouncementPanel,BeforeGameStartedPanel;
    [SerializeField] float TimerClock = 600f;
    [SerializeField] float TimeBeforeStart = 10f;
    const int WinThreshold = 5;
    

    int RedScore=0, BlueScore=0;

    private void Awake()
    {
        Instance = this;
        //if(NetworkManager.Singleton.IsServer)
        //{
        //    GetComponent<NetworkObject>().Spawn();
        //}
    }

    [ServerRpc (RequireOwnership =false)]
    public void SetTeamScoreScoreServerRpc(int score, bool isRed = true)
    {
        if (isRed)
            RedScore += score;
        else
            BlueScore += score;
        UpdateTeamScoresClientRpc(RedScore,BlueScore);
        LookForWinner();

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
       WinnerAnnouncementPanel.SetActive(true);
       if (isRedteamWon)
        {
            WinnerTitle.text = "Red Team Victory";
        }
       else
        {
            WinnerTitle.text = "Blue Team Victory";
        }
        AdmobAds.Instance.ShowInterstitialAd();
    }

    [ClientRpc]
    internal void UpdateTeamScoresClientRpc(int redScore,int blueScore)
    {
        RedTeamScore.text = redScore.ToString();
        BlueTeamScore.text = blueScore.ToString();
    }

    private void FixedUpdate()
    {
        if(IsServer)
        {
            if(TimeBeforeStart>0)
            {
                TimeBeforeStart -= Time.deltaTime;
                UpdateStartTime(TimeBeforeStart);
            }
            else if(StartTimeLabel.gameObject.activeSelf)
            {
                LobbyManager.Instance.GameHasStarted = true;
                DisableTimerStart();
            }
            if(LobbyManager.Instance.GameHasStarted)
            {
                TimerClock -= Time.deltaTime;
                UpdateTimer(TimerClock);
            }
        }
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
       
        UpdateTimerClientRpc(time);
    }

    public void UpdateStartTime(float time)
    {
        UpdateStartTimerClientRpc(time);
       
    }

    [ClientRpc]
    public void UpdateTimerClientRpc(float time)
    {
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
        PlayerSetManager.instance.ChangeView(false);
        PlayerSetManager.instance.SpinTheWheel();
    }
}
