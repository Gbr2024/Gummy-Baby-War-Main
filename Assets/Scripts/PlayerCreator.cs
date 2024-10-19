using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using WeirdBrothers.ThirdPersonController;

public class PlayerCreator : NetworkBehaviour
{
    public static PlayerCreator Instance;


    [Header("Sync")]
    public NetworkVariable<bool> isRedTeam = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> kills = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<FixedString64Bytes> playername = new NetworkVariable<FixedString64Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    internal int killstreak = 0;

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
        
        isRedTeam.OnValueChanged += (previous, current) => SetisRed(isRedTeam.Value);
        kills.OnValueChanged += (previous, current) => Setkills(kills.Value);
        playername.OnValueChanged += (previous, current) => SetName(playername.Value);
        if(IsOwner)
        {
            playername.Value = PlayerPrefs.GetString("PlayerName");
        }
        if(IsServer)
        {
            Invoke(nameof(CloseLobby), 30f);
        }
       
    }



    void CloseLobby()
    {
        LobbyManager.Instance.CloseLobby();
    }

    internal void SetIsRed()
    {
        if (IsOwner)
        {
            //Debug.LogError("Setting is Red" + CustomProperties.Instance.isRed);
            isRedTeam.Value = CustomProperties.Instance.isRed;
            //Debug.LogError("Setting is Red" + isRedTeam.Value);
        }
    }



    private void SetisRed(bool value)
    {
        isRedTeam.Value = value;
    }

    private void Setkills(int value)
    {
        kills.Value = value;
    }

    private void SetName(FixedString64Bytes value)
    {
        playername.Value = value;
    }

    //[ServerRpc (RequireOwnership =false)]
    //internal void setkillServerRpc(ulong id)
    //{
    //    SetKillClientRpc(id);
    //}

    [ClientRpc]
    internal void UpdateKillsClientRpc()
    {
        if(IsLocalPlayer)
        {
            UpdateKills();
        }
        
    }


    void UpdateKills()
    {
        ScoreManager.Instance.SetTeamScoreScoreServerRpc(1, isRedTeam.Value);
        kills.Value += 1;
        CustomProperties.Instance.kills = kills.Value;
        killstreak++;
        if (killstreak > 5) killstreak = 5;
        if (killstreak >= 2)
        { 
            WBUIActions.EnableKillstreakButton?.Invoke(true);
            
            WBUIActions.ChangeKillstreak?.Invoke(killstreak.ToString());
        }
        WBUIActions.UpdatelocalScore?.Invoke(kills.Value);

    }

    [ServerRpc(RequireOwnership = false)]
    public void OnClientConnectedServerRpc()
    {
        //Debug.LogError("Being Called " + NetworkManager.Singleton.ConnectedClients.Count );
        if(!LobbyManager.Instance.GameSceneHasLoaded && NetworkManager.Singleton.ConnectedClients.Count>=2)
        {
            Loader.LoadNetwork(PlayerPrefs.GetString("Level"));
        }
    }


    [ServerRpc(RequireOwnership = false)]
    void SpawnPlayerServerRpc(ulong id,int charindex,int index,int bulletLayer,bool isRed)
    {
        var t=isRed ? PlayerSetManager.instance.RedCribs[index] : PlayerSetManager.instance.BlueCribs[index];
        ItemReference.Instance.characters.Characters[charindex].transform.position = t.position;
        ItemReference.Instance.characters.Characters[charindex].transform.forward = t.forward;

        GameObject player = NetworkManager.Instantiate(ItemReference.Instance.characters.Characters[charindex],t.position,t.rotation);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(id, true);
        player.GetComponent<WBThirdPersonController>().bulletlayer = bulletLayer;
        player.transform.forward = t.forward;
       
    }
    
    [ServerRpc(RequireOwnership = false)]
    internal void SpawnAIServerRpc(string name="",bool isRed=true)
    {
        Transform t = isRed!=true?PlayerSetManager.instance.BlueCribs[Random.Range(0, PlayerSetManager.instance.BlueCribs.Length)]: PlayerSetManager.instance.RedCribs[Random.Range(0, PlayerSetManager.instance.BlueCribs.Length)];
        GameObject player = NetworkManager.Instantiate(PlayerSetManager.instance.AIPrefabs[Random.Range(0, PlayerSetManager.instance.AIPrefabs.Length)].gameObject,t.position ,t.rotation);
        player.GetComponent<NetworkObject>().Spawn(true);
        player.GetComponent<PlayerController>().bulletlayer.Value = isRed ? 9 : 12;
        player.GetComponent<PlayerController>().isRed.Value = isRed;
        
       
        if (string.IsNullOrEmpty(name))
        {
            player.GetComponent<PlayerController>().AIname = "Guest"+GenerateRandomNumberString(8);
            
            var aicreater=NetworkManager.Instantiate(ItemReference.Instance.AIcreator);
            aicreater.GetComponent<NetworkObject>().Spawn(true);
            aicreater.AIname.Value = player.GetComponent<PlayerController>().AIname;
            aicreater.isRed.Value = isRed;
        }
        else
        {
            player.GetComponent<PlayerController>().AIname = name;
        }
    }

    string GenerateRandomNumberString(int length)
    {
        string result = "";
        System.Random random = new System.Random();

        for (int i = 0; i < length; i++)
        {
            result += random.Next(0, 10).ToString(); // Generating random digits (0-9)
        }

        return result;
    }


    internal void SpawnObject()
    {
        int bulletlayer = 9;
        if (!CustomProperties.Instance.isRed)
            bulletlayer = 12;
        int index = Random.Range(0, PlayerSetManager.instance.RedCribs.Length);
        var tmp= CustomProperties.Instance.isRed ? PlayerSetManager.instance.RedCribs[index] :PlayerSetManager.instance.BlueCribs[index];

        ItemReference.Instance.characters.Characters[PlayerPrefs.GetInt("CharacterIndex", 0)].transform.position = tmp.transform.position;
        ItemReference.Instance.characters.Characters[PlayerPrefs.GetInt("CharacterIndex", 0)].transform.forward = tmp.transform.forward;
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, PlayerPrefs.GetInt("CharacterIndex", 0),index,bulletlayer, CustomProperties.Instance.isRed);
        killstreak = 0;
        WBUIActions.EnableKillstreakButton?.Invoke(false);
        WBUIActions.ChangeKillstreak?.Invoke(0.ToString());

    }



    [ServerRpc]
    internal void DespawnGrenadeServerRpc(ulong ownerClientId)
    {
        foreach (var item in FindObjectsOfType<Grenade>())
        {
            item.NetworkObject.Despawn(true);
        } 
    }

    [ServerRpc (RequireOwnership =false)]
    internal void CreateGrenadeServerRpc(ulong ownerClientId,bool isRed)
    {
        WBThirdPersonController controller=null;
        foreach (var item in FindObjectsOfType<WBThirdPersonController>())
        {
            if (item.OwnerClientId == ownerClientId)
                controller = item;
        }
        controller.Context.trajectory.prefab.transform.position = controller.Context.GrenadeHandPos.position;
        Grenade grenade = NetworkManager.Instantiate(controller.Context.trajectory.prefab);
        grenade.ToFollow = controller.Context.GrenadeHandPos;
        grenade.PlayerID = ownerClientId;
        grenade.isRed = isRed;
        controller.Context.trajectory.grenade = grenade;
        grenade.GetComponent<NetworkObject>().SpawnWithOwnership(ownerClientId, true);
    }

   
}
