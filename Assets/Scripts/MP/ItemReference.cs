using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReference :MonoBehaviour
{
    public static ItemReference Instance;
    public WeaponsData weaponsData;
    public ColorReference colorReference;
    public CharactersData characters;
    public Transform EmtptyTarget;
    public float hasgoneDownY = -10f;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Instance = this;
    }
}
