using EasyUI.PickerWheelUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSetter : MonoBehaviour
{
    [SerializeField] PickerWheel pickerWheel;
    [SerializeField] WheelPiece[] wheelPieces;

   
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("isShotGunUnlocked")==1)
        {
            foreach (var item in wheelPieces)
            {
                if(item.Label== "Shotgun" && !pickerWheel.wheelPieces.Contains(item))
                {
                    pickerWheel.wheelPieces.Add(item);
                }
            }
            
        }
    }

    
}
