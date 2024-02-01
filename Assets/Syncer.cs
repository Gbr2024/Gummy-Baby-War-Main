using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using WeirdBrothers.ThirdPersonController;
using System;

public class Syncer : NetworkBehaviour
{
    WBThirdPersonController controller;
    public NetworkVariable<int> SkinColor = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Vector3> SpineIK = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Vector3> SpineRot = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
       
        // subscribe on value changed, or send a clientRpc. In this case subscribe.
        SkinColor.OnValueChanged += (previous, current) => SetSkin(SkinColor.Value);
        SpineIK.OnValueChanged += (previous, current) => SetSpine(SpineIK.Value);
        SpineRot.OnValueChanged += (previous, current) => SetSpineRot(SpineRot.Value);
        // To immediately sync for late join players.
        controller = GetComponent<WBThirdPersonController>();

        if (IsOwner)
            SkinColor.Value = PlayerPrefs.GetInt("ColorIndex", 0);
        else
            SetSkin(SkinColor.Value);
        //SkinColor.OnValueChanged.Invoke(0, );
    }

    private void SetSpine(Vector3 value)
    {
        //print("here "+gameObject.name +"  " + value);
        
         controller.Context.RpcLookPos=value;
    }
    private void SetSpineRot(Vector3 value)
    {
        //print("here "+gameObject.name +"  " + value);
        
         controller.Context.RpcSpineRotation=value;
    }

    public override void OnNetworkDespawn()
    {
        SkinColor.OnValueChanged -= (previous, current) => SetSkin(SkinColor.Value);
        base.OnNetworkDespawn();
    }

    public void LateUpdate()
    {
        if (IsOwner && controller != null)
        {
            SpineIK.Value = controller.Context.WeaponIK.LookAt.position;
            SpineRot.Value = controller.Context.WeaponIK.SpineRotation;
        }
    }

    public void SetSkin(int color)
    {
        Debug.LogError("On Value Invoked +" + gameObject.name);
        List<Material> mats = new List<Material>();
        foreach (var item in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            foreach (var item2 in item.materials)
            {
                mats.Add(item2);
            }
        }
        foreach (var item in mats)
        {
            item.SetColor("_BaseColor", ItemReference.Instance.colorReference.CharacterColors[color].color);
        }
        
        //spriterenderer.DOColor(color, time);
    }

}
