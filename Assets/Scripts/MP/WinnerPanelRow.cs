using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinnerPanelRow : MonoBehaviour
{
    [SerializeField] TMP_Text Name, kills;
    [SerializeField] Image Bg;

    public void SetData(PlayerData playerData,bool makeYellow)
    {
        if(makeYellow)
        {
            Color c = Color.yellow;
            c.a = Bg.color.a;
            Bg.color = c;
        }
        Name.text = playerData.Name;
        kills.text = playerData.Kills.ToString();
        gameObject.SetActive(true);
    }
}
