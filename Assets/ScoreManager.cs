using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ScoreManager : NetworkBehaviour
{
    [SerializeField] TMP_Text RedTeamScore, BlueTeamScore,WinnerTitle,TimerLabel,Mykills;
    [SerializeField] GameObject WinnerAnnouncementPanel;
    [SerializeField] float TimerClock = 600f;
    const int WinThreshold = 5;
    

    int RedScore=0, BlueScore=0;

    private void Awake()
    {
        if(NetworkManager.Singleton.IsServer)
        {
            GetComponent<NetworkObject>().Spawn();
        }
    }

    [ServerRpc]
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
        if (RedScore>WinThreshold)
        {
            ShowWinnerClientRpc();
        }
        else if(BlueScore > WinThreshold)
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
            TimerClock -= Time.deltaTime;
            UpdateTimer(TimerClock);
        }
    }

    
    public void UpdateTimer(float time)
    {
        UpdateTimerClientRpc(time);
    }

    [ClientRpc]
    public void UpdateTimerClientRpc(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        TimerLabel.text = minutes.ToString("00") + seconds.ToString("00");
    }

    [ClientRpc]
    public void UpdateMykillsClientRpc(int kills)
    {
        Mykills.text = kills.ToString();
    }
}
