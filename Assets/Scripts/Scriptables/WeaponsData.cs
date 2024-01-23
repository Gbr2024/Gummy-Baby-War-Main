using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponsData", menuName = "Data/Weapons")]
public class WeaponsData : ScriptableObject
{
    public GameObject[] Weapons;
    public GameObject[] WeaponsOnShow;
}
