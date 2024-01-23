using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemReference :NetworkBehaviour
{
    public static ItemReference Instance;
    public WeaponsData weaponsData;
    public ColorReference colorReference;
    public CharactersData characters;

    private void Awake()
    {
        Instance = this;
        
    }
}
