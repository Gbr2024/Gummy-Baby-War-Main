using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShotGunReward : MonoBehaviour
{
    [SerializeField] string GameLink = "www.google.com";
    [SerializeField] TMP_Text Times;

    int MaxTime = 30;
    string ShotGunPlayerPref = "isShotGunUnlocked";
    string ShotGunSharePlayerPref= "ShotGunShare";

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt(ShotGunSharePlayerPref, 0) >= MaxTime)
        {
            Times.text = "Claimed";
            GetComponent<Button>().interactable = false;
        }
        else
            Times.text = PlayerPrefs.GetInt(ShotGunSharePlayerPref, 0).ToString()+"/"+MaxTime;
    }

    public void ShareGame()
    {
        new NativeShare()
        .SetSubject("Gummy Baby War!").SetText("Hey mate, check this game out.").SetUrl(GameLink)
        .SetCallback((result, shareTarget) => Onsuccess(result,shareTarget))
        .Share();
    }

    private void Onsuccess(NativeShare.ShareResult result, string shareTarget)
    {
        if(result==NativeShare.ShareResult.Shared)
        {
            PlayerPrefs.SetInt(ShotGunSharePlayerPref, PlayerPrefs.GetInt(ShotGunSharePlayerPref, 0) + 1);
            if (PlayerPrefs.GetInt(ShotGunSharePlayerPref, 0) >= MaxTime)
            {
                Times.text = "Claimed";
                GetComponent<Button>().interactable = false;
                PlayerPrefs.SetInt(ShotGunPlayerPref, 1);
            }
            else
                Times.text = PlayerPrefs.GetInt(ShotGunSharePlayerPref, 0).ToString() + "/" + MaxTime;

        }
    }
}
