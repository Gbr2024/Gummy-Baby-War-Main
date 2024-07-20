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
    public AICreater AIcreator;
    public float hasgoneDownY = -10f;
    public Vector2 dropMin, DropMax;
    public float SightRange, AttackRange;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Instance = this;
    }
}
