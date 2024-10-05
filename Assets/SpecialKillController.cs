using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using DG.Tweening;
using WeirdBrothers;
using WeirdBrothers.ThirdPersonController;
using Cinemachine;

public class SpecialKillController : NetworkBehaviour
{
    public List<string> SpecialKill = new();
    public NetworkVariable<bool> DummyActivated = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] GameObject DummyRocket;

    WBThirdPersonController controller;
    [SerializeField] Impact impact;

    Transform target = null;

    Animator animator;

    //[SerializeField] Vector3 DummyRocketPose;
    // Start is called before the first frame update
    void Start()
    {
        //DummyRocket.transform.localPosition = DummyRocketPose;
        animator = GetComponent<Animator>();
        controller = GetComponent<WBThirdPersonController>();
    }

    public void SetKills()
    {
        
        WBUIActions.SetSpecialKill(SpecialKill);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        DummyActivated.OnValueChanged += (previous, current) => SetActivated(DummyActivated.Value);
    }

    private void SetActivated(bool value)
    {
        DummyRocket.SetActive(value);

    }

    internal void InvokeKill(string obj)
    {
        if (!LobbyManager.Instance.GameHasStarted) return;

        switch (obj)
        {
            case "Granny":
                SetGrannyforTeamServerRpc();
                break;
            default:
                SetRocket();
                break;
        }

    }

    Vector3 pos;

    private void SetRocket()
    {
        target=null;
        animator.SetBool("StopFlying", false);
        animator.SetTrigger("Fly");
        foreach (var item in FindObjectsOfType<WBThirdPersonController>())
        {
            if(item.isRed!=controller.isRed)
            {
               target = item.transform;
                break;
            }
        }
        if (target== null)
        {
            foreach (var item in FindObjectsOfType<PlayerController>())
            {
                if (item.isRed.Value != controller.isRed)
                {
                    target = item.transform;
                    break;
                }
            }
        }
        if (target== null) return;

        PlayerSetManager.instance.virtualCamera.gameObject.SetActive(false);
        PlayerSetManager.instance.RocketCamera.gameObject.SetActive(true);
        pos = target.position;
        DummyActivated.Value = true;
        controller.RestrictInput();
        var tpos=new Vector3(transform.position.x, 75f, transform.position.z);
        transform.DOLookAt(tpos, .35f);
        transform.DOMove(tpos, 5f).OnComplete(() => { Invoke(nameof(SetRocketTarget), .35f); });
    }

    void SetRocketTarget()
    {
        if(target!=null)
            pos = target.position;
        float time = 1f * (Vector3.Distance(transform.position, pos) / 50f);
        transform.DOLookAt(pos, .35f);
        transform.DOMove(pos, time).OnUpdate(()=> { 
            if(Vector3.Distance(transform.position, pos)<2f)
            {
                DummyActivated.Value = false;
                BlastServerRpc(OwnerClientId, controller.isRed, pos);
                animator.SetBool("StopFlying", true);
            }
        }).OnComplete(() => { 
            controller.DeactiveRestriction();
            PlayerSetManager.instance.virtualCamera.gameObject.SetActive(true);
            PlayerSetManager.instance.RocketCamera.gameObject.SetActive(false);
        });

    }

    [ServerRpc (RequireOwnership =false)]
    void SetGrannyforTeamServerRpc()
    {
        KillStreakSystem.Instance.SpawnTeamGranny(OwnerClientId, GetComponent<Syncer>().isRed.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void BlastServerRpc(ulong id, bool isRed,Vector3 pos)
    {
        impact.transform.position = pos;
        var effect = NetworkManager.Instantiate(impact).GetComponent<Impact>();
        
        effect.NetworkObject.SpawnWithOwnership(id, true);
        effect.PlayerID.Value = id;
        effect.isRed.Value = isRed;

    }
}
