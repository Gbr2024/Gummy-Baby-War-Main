using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using WeirdBrothers.ThirdPersonController;
using UnityEngine.UI;
using EasyUI.PickerWheelUI;
using Unity.Netcode;
using System;

public class PlayerSetManager : MonoBehaviour
{
    public static PlayerSetManager instance;
    [SerializeField] GameObject InputControl;
    [SerializeField] RectTransform CrossHair, CrossHairScope;
    [SerializeField] Transform lookTransform;
    public PlayerController[] AIPrefabs;
    public CinemachineVirtualCamera virtualCamera;//,ScopeCinemachine;
    [SerializeField] Button Aimbutton;

    public PickerWheel wheel;

    public Transform[] RedCribs, BlueCribs;

    private void Awake()
    {
        instance = this;
        //Debug.LogError("instance Set");
        AdmobAds.Instance?.DestroyBannerAd();
    }

    

    // Start is called before the first frame update

    void Start()
    {
        Invoke(nameof(SpawnPlayer), 1f);
        PlayerCreator.Instance.SetIsRed();
        //SpinTheWheel();

    }

    private void OnspinComplete(WheelPiece obj)
    {
        WBUIActions.EnableBlackPanel?.Invoke(false);
        WBUIActions.isPlayerActive = true;
        PlayerPrefs.SetInt("WeaponIndex", obj.Amount);
        SpawnPlayer();
        wheel.transform.parent.gameObject.SetActive(false);
    }

    internal void spawnWithoutWheel()
    {
        WBUIActions.EnableBlackPanel?.Invoke(false);
        WBUIActions.isPlayerActive = true;
        SpawnPlayer();
        wheel.transform.parent.gameObject.SetActive(false);
    }

    public void setPlayerControlandCam(GameObject go)
    {
        //go.GetComponent<WBThirdPersonController>().Context.CrossHair.CrossHair = CrossHair;
        //go.GetComponent<WBThirdPersonController>().SetWeaponData();
        virtualCamera.LookAt = go.transform;
        virtualCamera.Follow = go.transform;
        go.GetComponent<WBInputHandler>().SetInput(InputControl);

        //Debug.LogError("data Set");
    }

    internal void SetCamera(WBThirdPersonController thirdPersonController)
    {
        virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = 0;
        virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = 0;
        thirdPersonController.Context.setcamera(virtualCamera);
        thirdPersonController.Context.CrossHair .CrossHair= CrossHair;
        thirdPersonController.Context.WeaponIK.LookAt = lookTransform;
       
    }


    public void SpawnPlayer()
    {
        WBUIActions.EnableSecShoot?.Invoke(false);
        PlayerCreator.Instance.SpawnObject();
    }

    internal void setAimCam(WBThirdPersonController thirdPersonController)
    {
        Aimbutton.onClick.RemoveAllListeners();
        Aimbutton.onClick.AddListener(() => { thirdPersonController.SetScope(!thirdPersonController.Context.isScopeOn); });
        Debug.LogError("Cameras has set" + thirdPersonController.gameObject.name);
    }

    internal bool scopemoving = false;
    
    public void ChangeView(float fov)
    {
        if (scopemoving) return;
        DOTween.To(() => virtualCamera.m_Lens.FieldOfView, x => virtualCamera.m_Lens.FieldOfView = x, fov, .35f)
            .OnUpdate(() => {
                // This will be called every frame during the animation
                scopemoving = true;
            })
            .OnComplete(() => {
                // This will be called when the animation is complete
                scopemoving = false;
            });
        //virtualCamera.
    }


    internal void SpinTheWheel()
    {
        WBUIActions.EnableBlackPanel?.Invoke(false);
        wheel.transform.parent.gameObject.SetActive(true);
        wheel.Spin();
        wheel.OnSpinEnd(wheelPiece => { OnspinComplete(wheelPiece); });
    }

    internal void setKillCam(Transform t)
    {
        if (t == null) return;
        WBUIActions.isPlayerActive = false;
        WBUIActions.EnableBlackPanel?.Invoke(true);
        virtualCamera.Follow = t;
        virtualCamera.LookAt = t;
        virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = 0;
        virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = 0;
    }

    internal void CreateNewAI(string name="",bool isRed=true)
    {
        PlayerCreator.Instance.SpawnAIServerRpc(name,isRed);
    }
}
