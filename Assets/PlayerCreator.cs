using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using WeirdBrothers.ThirdPersonController;

public class PlayerCreator : NetworkBehaviour
{
    public static PlayerCreator Instance;
    private void Awake()
    {
       
        

    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError("-------" + IsOwner);
        if (IsOwner)
            Instance = this;
        DontDestroyOnLoad(gameObject);
        //SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId,PlayerPrefs.GetInt("CharacterIndex", 0));
        //GameObject go = NetworkManager.Instantiate(ItemReference.Instance.characters.Characters[PlayerPrefs.GetInt("CharacterIndex", 0)], Vector3.zero, Quaternion.identity);
        //go.GetComponent<NetworkObject>().Spawn(true);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnPlayerServerRpc(ulong id,int charindex,int weaponIndex)
    {
        Debug.LogError(charindex);
        //PlayerPrefs.GetInt("CharacterIndex", 0);
        //PlayerPrefs.GetInt("WeaponIndex", 0);
        //PlayerPrefs.GetInt("ColorIndex", 0);
        // Instantiate the player prefab on the server
        GameObject player = NetworkManager.Instantiate(ItemReference.Instance.characters.Characters[charindex], Vector3.zero, Quaternion.identity);
        GameObject Weapon = NetworkManager.Instantiate(ItemReference.Instance.weaponsData.Weapons[weaponIndex],player.transform) ;
        Weapon.transform.position = Vector3.zero;
        Weapon.transform.rotation = Quaternion.identity;
        // Spawn the player object on all clients
        player.GetComponent<NetworkObject>().SpawnWithOwnership(id, true);
        Weapon.GetComponent<NetworkObject>().Spawn(true);
        //player.GetComponent<NetworkObject>().
    }

    internal void SpawnObject()
    {
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId,Random.Range(0,7), PlayerPrefs.GetInt("WeaponIndex", 0));

    }
}
