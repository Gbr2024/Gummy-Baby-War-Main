using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
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
      
        DontDestroyOnLoad(gameObject);
        //SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId,PlayerPrefs.GetInt("CharacterIndex", 0));
        //GameObject go = NetworkManager.Instantiate(ItemReference.Instance.characters.Characters[PlayerPrefs.GetInt("CharacterIndex", 0)], Vector3.zero, Quaternion.identity);
        //go.GetComponent<NetworkObject>().Spawn(true);
        
        
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
            Instance = this;
        if (IsOwner) OnClientConnectedServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnClientConnectedServerRpc()
    {
        //Debug.LogError("Being Called " + NetworkManager.Singleton.ConnectedClients.Count );
        if(!LobbyManager.Instance.GameSceneHasLoaded && NetworkManager.Singleton.ConnectedClients.Count>=2)
        {
            Loader.LoadNetwork(Loader.Gamescenes[Random.Range(0, Loader.Gamescenes.Length)]);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    void SpawnPlayerServerRpc(ulong id,int charindex,int weaponIndex,Vector3 pos,int attacklayer)
    {
        //PlayerPrefs.GetInt("CharacterIndex", 0);
        Debug.LogError(pos);
        //PlayerPrefs.GetInt("WeaponIndex", 0);
        //PlayerPrefs.GetInt("ColorIndex", 0);
        // Instantiate the player prefab on the server
        GameObject player = NetworkManager.Instantiate(ItemReference.Instance.characters.Characters[charindex],pos,Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(id, true);
        //player.GetComponent<WBThirdPersonController>().SetWeaponDataClientRpc(id, weaponIndex);
        //if(IsServer)
        
        player.GetComponent<WBThirdPersonController>().SetWeaponDataServerRpc(id, weaponIndex,attacklayer);
        //player.GetComponent<WBThirdPersonController>().SetColorServerRpc(id, ColorIndex);
        //Weapon.transform.position = Vector3.zero;
        //Weapon.transform.rotation = Quaternion.identity;
        // Spawn the player object on all clients

        //Weapon.GetComponent<NetworkObject>().Spawn(true);
        //player.GetComponent<NetworkObject>().
    }

    internal void SpawnObject()
    {
        int bulletlayer = 9;
        if (!CustomProperties.Instance.isRed)
            bulletlayer = 12;
        Vector3 pos=CustomProperties.Instance.isRed?transform.position = PlayerSetManager.instance.RedCribs[Random.Range(0, 3)].position: transform.position = PlayerSetManager.instance.BlueCribs[Random.Range(0, 3)].position; ;
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, PlayerPrefs.GetInt("CharacterIndex", 0), PlayerPrefs.GetInt("WeaponIndex", 0),pos,bulletlayer);
    }

    
}
