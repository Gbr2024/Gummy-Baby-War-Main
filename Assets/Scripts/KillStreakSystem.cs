using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using PathCreation;
using WeirdBrothers.ThirdPersonController;

public class KillStreakSystem : NetworkBehaviour
{
    public static KillStreakSystem Instance;
    [SerializeField] Vector2 dropMin, DropMax;
    [SerializeField] float restTime = 20f;
    [SerializeField] LayerMask layertocheck;
    [SerializeField] Transform[] EndRidges;
    [SerializeField] GameObject Drop;
    [SerializeField] FlyController Chopperprefab;
    [SerializeField] PlanerController planePrefab;
    [SerializeField] PathCreator Chopperpath;
    [SerializeField] PegionController pegionController;
    [SerializeField] int testNumber = 0;
    [SerializeField] bool test,SpawnGranny;
    [SerializeField] Transform[] GrannyPos;
    [SerializeField] Granny grannyPrefab;

    // Start is called before the first frame update

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        // Ensure that the owner client is the server before invoking the spawn coroutine
        

    }

   

    // Coroutine to spawn drops periodically
    IEnumerator SpawnDropsPeriodically()
    {
        while (!ScoreManager.Instance.GameHasFinished)
        {
            yield return new WaitForSeconds(restTime); // Wait for 30 seconds before spawning next drop

            // Check if the owner client is still the server (in case ownership changes during runtime)
            // Generate random position within the defined range in XZ plane
            Vector3 spawnPosition = new Vector3(Random.Range(dropMin.x, DropMax.x), 75f, Random.Range(dropMin.y, DropMax.y));

            //while (!IsGroundUnderneath(spawnPosition))
            //{
            //    Debug.LogError("Here 30");
            //    spawnPosition = new Vector3(Random.Range(dropMin.x, DropMax.x), 75f, Random.Range(dropMin.y, DropMax.y));
            //    yield return null;
            //}

            // Check if the spawn position is valid (not colliding with any objects in the specified layer)
            // Spawn the drop prefab at the calculated position
            Drop.transform.position = spawnPosition;
            Crate go = NetworkManager.Instantiate(Drop, spawnPosition, Quaternion.identity).GetComponent<Crate>();
            go.NetworkObject.SpawnWithOwnership(OwnerClientId,true);
        }
    }

    internal void SetCrates()
    {
        StartCoroutine(SpawnDropsPeriodically());
    }

    internal void SetGranny()
    {
        if(SpawnGranny)
        {
            grannyPrefab.transform.position = GrannyPos[Random.Range(0, GrannyPos.Length)].position;
            Granny go = NetworkManager.Instantiate(grannyPrefab, grannyPrefab.transform.position, Quaternion.identity).GetComponent<Granny>();
            go.NetworkObject.Spawn(true);
        }
    }

    private bool IsGroundUnderneath(Vector3 spawnPosition)
    {
        // Raycast downwards to check for ground
        Debug.LogError(spawnPosition);
        RaycastHit hit;
        if (Physics.Raycast(spawnPosition, Vector3.down, out hit, Mathf.Infinity, layertocheck))
        {
            Debug.LogError(hit.collider.gameObject.layer);
            // Check if the hit object is part of the ground layer
            return hit.collider.gameObject.layer == layertocheck;
        }
        return false;
    }

    internal void SetKillstreak(ulong id,bool isred,int index)
    {
        switch(index)
        {
            case 2:
                SetMap();
                break;
            case 3:
                SetPlaneandTargetServerRpc(id, isred);
                break;
            case 4:
                SetPlgeoonandTargetServerRpc(id, isred);
                break;
            case 5:
                SetChopperandTargetServerRpc(id, isred);
                break;
        }
        WBUIActions.EnableKillstreakButton?.Invoke(false);
    }

    private void SetMap()
    {
        
        foreach (var item in FindObjectsOfType<WBThirdPersonController>())
        {
            if (item.isRed != CustomProperties.Instance.isRed)
            {
                item.GetComponentInChildren<marker>().EnableBody(true);
            }
        }
        foreach (var item in FindObjectsOfType<PlayerController>())
        {
            if (item.isRed.Value != CustomProperties.Instance.isRed)
            {
                item.GetComponentInChildren<marker>().EnableBody(true);
            }
        }
        WBUIActions.EnableMap(true);
     }

    [ServerRpc (RequireOwnership =false)]
    internal void SetChopperandTargetServerRpc(ulong id,bool isRed)
    {
        if (!IsServer) return;
        START:
        var item = GetRandomObject(!isRed);

        if (item != null)
        {
            Chopperpath.transform.position = new Vector3(item.transform.position.x, 30f, item.transform.position.z);
            var path = Instantiate(Chopperpath);
            var Chopper = NetworkManager.Instantiate(Chopperprefab);
            Chopper.NetworkObject.SpawnWithOwnership(OwnerClientId,true);
            Chopper.SetTargetClientRpc(item.OwnerClientId, id, isRed);
            Chopper.SetPath(path);
            return;
        }
        var item1 = GetAI(!isRed);
        if (item1 != null)
        {
            Chopperpath.transform.position = new Vector3(item1.transform.position.x, 30f, item1.transform.position.z);
            var path = Instantiate(Chopperpath);
            var Chopper = NetworkManager.Instantiate(Chopperprefab);
            Chopper.NetworkObject.SpawnWithOwnership(OwnerClientId, true);
            Chopper.SetTargetClientRpc(id, item1.OwnerClientId, isRed,true,item1.AIname);
            Chopper.SetPath(path);
            
        }
        else if(!ScoreManager.Instance.GameHasFinished)
            goto START;
    }
    
    
    
    [ServerRpc (RequireOwnership =false)]
    internal void SetPlaneandTargetServerRpc(ulong id,bool isRed)
    {
        if(!IsServer) return;
        var item = GetRandomObject(!isRed);
        planePrefab.transform.position = EndRidges[Random.Range(0,EndRidges.Length)].position;
        if (item != null)
        {
            var plane = NetworkManager.Instantiate(planePrefab);
            plane.NetworkObject.SpawnWithOwnership(OwnerClientId, true);
            plane.SetTargetClientRpc(id, item.OwnerClientId, isRed);
            plane.SetTarget(item.transform.position);
            return;
        }
        var item1 = GetAI(!isRed);
        if(item1!=null)
        {
            var plane = NetworkManager.Instantiate(planePrefab);
            plane.NetworkObject.SpawnWithOwnership(OwnerClientId, true);
            plane.SetTargetClientRpc(id, item1.OwnerClientId, isRed,true,item1.AIname);
            plane.SetTarget(item1.transform.position,true);
        }
    }
    
    [ServerRpc (RequireOwnership =false)]
    internal void SetPlgeoonandTargetServerRpc(ulong id,bool isRed)
    {
        if (!IsServer) return;
        var item = GetRandomObject(!isRed);
        pegionController.transform.position = EndRidges[Random.Range(0, EndRidges.Length)].position;
        if (item!=null)
        {
            var plane =NetworkManager.Instantiate(pegionController);
            plane.NetworkObject.SpawnWithOwnership(OwnerClientId, true);
            plane.SetTargetClientRpc(id, item.OwnerClientId, isRed);
            plane.SetTarget(item.transform);
            return;
        }
        var item1 = GetAI(!isRed);
        if (item1 != null)
        {
            var plane = NetworkManager.Instantiate(pegionController);
            plane.NetworkObject.SpawnWithOwnership(OwnerClientId, true);
            plane.SetTargetClientRpc(id, item1.OwnerClientId, isRed,true,item1.AIname);
            plane.SetTarget(item1.transform, true);
        }
    }

    public WBThirdPersonController GetRandomObject(bool v)
    {
        var l = FindObjectsOfType<WBThirdPersonController>();
        foreach (var item in l)
        {
            if (item.isRed == v)
                return item;
        }
        return null;
    }
    
    public PlayerController GetAI(bool v)
    {
        var l = FindObjectsOfType<PlayerController>();
        foreach (var item in l)
        {
            if (item.isRed.Value == v)
                return item;
        }
        return null;
    }

    private void FixedUpdate()
    {
        if(test)
        {
            test = false;
            SetKillstreak(NetworkManager.LocalClientId, CustomProperties.Instance.isRed, testNumber);
        }
    }

    
}
