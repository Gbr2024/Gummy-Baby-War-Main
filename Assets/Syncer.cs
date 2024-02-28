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
    public NetworkVariable<bool> Activated = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isRed = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> WeaponIndex = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // subscribe on value changed, or send a clientRpc. In this case subscribe.
        SkinColor.OnValueChanged += (previous, current) => SetSkin(SkinColor.Value);
        SpineIK.OnValueChanged += (previous, current) => SetSpine(SpineIK.Value);
        SpineRot.OnValueChanged += (previous, current) => SetSpineRot(SpineRot.Value);
        Activated.OnValueChanged += (previous, current) => SetActivated(Activated.Value);
        isRed.OnValueChanged += (previous, current) => SetisRed(Activated.Value);
        WeaponIndex.OnValueChanged += (previous, current) => SetWeaponIndex(WeaponIndex.Value);

        // To immediately sync for late join players.
        controller = GetComponent<WBThirdPersonController>();

        if (IsOwner)
        {
            SkinColor.Value = PlayerPrefs.GetInt("ColorIndex", 0);
           
        }
        else
        {
            SetSkin(SkinColor.Value);
            SetActivated(Activated.Value);
            SetWeaponIndex(WeaponIndex.Value);
            SetisRed(isRed.Value);
        }
    }

    private void SetisRed(bool value)
    {
        controller.isRed = value;
        gameObject.layer = isRed.Value ? 10 : 13;
        controller.bulletlayer = controller.isRed ? 9 : 12;
    }

    private void SetSpine(Vector3 value)
    {
        controller.Context.RpcLookPos = value;
    }

    private void SetSpineRot(Vector3 value)
    {
        controller.Context.RpcSpineRotation = value;
    }

    // New method to set activation state
    private void SetActivated(bool value)
    {
        GetComponent<HealthManager>().isActivated = value;
    }

    // New method to set weapon index
    private void SetWeaponIndex(int value)
    {
        // Implement logic to change the weapon based on the index value
        //if(!IsOwner)
            controller.SetWeaponData(value, controller.bulletlayer);
    }

    public override void OnNetworkDespawn()
    {
        SkinColor.OnValueChanged -= (previous, current) => SetSkin(SkinColor.Value);
        Activated.OnValueChanged -= (previous, current) => SetActivated(Activated.Value);
        WeaponIndex.OnValueChanged -= (previous, current) => SetWeaponIndex(WeaponIndex.Value);
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
    }
}
