using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorData", menuName = "Data/ColorData")]
public class ColorReference : ScriptableObject
{
    public ColorReferences[] CharacterColors;
    public Material[] skyboxes;
    public int[] VideoIndex;
    public SpecialAttackImages[] specialAttacksImages;
   
}

[System.Serializable]
public class ColorReferences
{
    public string id;
    public Color color;
}

[System.Serializable]
public class SpecialAttackImages
{
    public string Id;
    public Sprite Image;
}