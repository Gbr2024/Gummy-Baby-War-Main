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
    [NonSerialized] public NetworkVariable<Vector3> SpineIK = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [NonSerialized] public NetworkVariable<Vector3> SpineRot = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> Activated = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isRed = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isWeaponActivated = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isAiming = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> WeaponIndex = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        
        // subscribe on value changed, or send a clientRpc. In this case subscribe.
        SkinColor.OnValueChanged += (previous, current) => controller.SetSkin(SkinColor.Value);
        SpineIK.OnValueChanged += (previous, current) => SetSpine(SpineIK.Value);
        SpineRot.OnValueChanged += (previous, current) => SetSpineRot(SpineRot.Value);
        Activated.OnValueChanged += (previous, current) => SetActivated(Activated.Value);
        isWeaponActivated.OnValueChanged += (previous, current) => SetWeaponActivated(isWeaponActivated.Value);
        isRed.OnValueChanged += (previous, current) => SetisRed(isRed.Value);
        isAiming.OnValueChanged += (previous, current) => SetisAim(isAiming.Value);
        WeaponIndex.OnValueChanged += (previous, current) => SetWeaponIndex(WeaponIndex.Value);

        // To immediately sync for late join players.
        controller = GetComponent<WBThirdPersonController>();

        if (IsOwner)
        {
            SkinColor.Value = PlayerPrefs.GetInt("ColorIndex", 0);
           
        }
        else
        {
            controller.SetSkin(SkinColor.Value);
            SetActivated(Activated.Value);
            SetWeaponIndex(WeaponIndex.Value);
            SetisRed(isRed.Value);
        }
    }

    private void SetisAim(bool value)
    {
        controller.Context.isAiming = value;
        controller.Context.Animator.SetAim(value);
    }

    private void SetWeaponActivated(bool value)
    {
        if(controller.Context.CurrentWeapon!=null)
            controller.Context.CurrentWeapon.Body.SetActive(value);
        controller.Context.GrenadeSet = !value;
    }

   

    private void SetisRed(bool value)
    {
        controller.isRed = value;
        gameObject.layer = isRed.Value ? 10 : 13;
        controller.bulletlayer = controller.isRed ? 9 : 12;
        controller.SetSkin(LobbyManager.Instance.getSkinColor(isRed.Value));
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
        SkinColor.OnValueChanged -= (previous, current) =>controller.SetSkin(SkinColor.Value);
        Activated.OnValueChanged -= (previous, current) => SetActivated(Activated.Value);
        WeaponIndex.OnValueChanged -= (previous, current) => SetWeaponIndex(WeaponIndex.Value);
        SpineIK.OnValueChanged -= (previous, current) => SetSpine(SpineIK.Value);
        SpineRot.OnValueChanged -= (previous, current) => SetSpineRot(SpineRot.Value);
        isRed.OnValueChanged -= (previous, current) => SetisRed(isRed.Value);
        isAiming.OnValueChanged -= (previous, current) => SetisAim(isAiming.Value);
        WeaponIndex.OnValueChanged -= (previous, current) => SetWeaponIndex(WeaponIndex.Value);
        base.OnNetworkDespawn();
    }

    public void LateUpdate()
    {
        if (IsOwner && controller != null)
        {
            SpineIK.Value = controller.Context.WeaponIK.LookAt.position;
            SpineRot.Value = controller.Context.WeaponIK.SpineRotation;
            if (controller.Context.CurrentWeapon != null) isWeaponActivated.Value = controller.Context.CurrentWeapon.Body.activeSelf;
            isAiming.Value = controller.Context.isAiming;
        }
    }
    
}

public static class SerializationExtensions
{
    public static void ReadValueSafe(this FastBufferReader reader, out Vector3[] value)
    {
        reader.ReadValueSafe(out Vector3[] val);
        value = val;
    }
}
