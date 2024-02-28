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
        //Debug.LogError("-------" + IsOwner);
      
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
    void SpawnPlayerServerRpc(ulong id,int charindex,Vector3 pos,int bulletLayer)
    {
        ItemReference.Instance.characters.Characters[charindex].transform.position = pos;
        GameObject player = NetworkManager.Instantiate(ItemReference.Instance.characters.Characters[charindex],pos,Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(id, true);
        player.GetComponent<WBThirdPersonController>().bulletlayer = bulletLayer;
    }

    internal void SpawnObject()
    {
        int bulletlayer = 9;
        if (!CustomProperties.Instance.isRed)
            bulletlayer = 12;
        Vector3 pos=CustomProperties.Instance.isRed?transform.position = PlayerSetManager.instance.RedCribs[Random.Range(0, PlayerSetManager.instance.RedCribs.Length)].position: transform.position = PlayerSetManager.instance.BlueCribs[Random.Range(0, PlayerSetManager.instance.BlueCribs.Length)].position;
        ItemReference.Instance.characters.Characters[PlayerPrefs.GetInt("CharacterIndex", 0)].transform.position = pos;
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, PlayerPrefs.GetInt("CharacterIndex", 0),pos,bulletlayer);
    }

    
}
