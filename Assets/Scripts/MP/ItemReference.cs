using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReference :MonoBehaviour
{
    public static ItemReference Instance;
    public WeaponsData weaponsData;
    public ColorReference colorReference;
    public CharactersData characters;
    public ChatScriptable chatData;
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

    internal AudioClip getclip(string message)
    {
        foreach (var item in chatData.chats)
        {
            if (item.ID == message) return item.Clip;
        }
        return null;
    }
    internal Chat GetChat(string message)
    {
        foreach (var item in chatData.chats)
        {
            if (item.ID == message) return item;
        }
        return null;
    }
}
