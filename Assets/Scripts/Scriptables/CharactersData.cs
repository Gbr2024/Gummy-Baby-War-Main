using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorData", menuName = "Data/Characters")]
public class CharactersData : ScriptableObject
{
    public GameObject[] Characters;
    public GameObject[] CharactersOnShow;
}
