using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using WeirdBrothers;
using WeirdBrothers.ThirdPersonController;
using UnityEngine.UI;
using EasyUI.PickerWheelUI;

public class PlayerSetManager : MonoBehaviour
{
    public static PlayerSetManager instance;
    [SerializeField] GameObject InputControl,SniperUI;
    [SerializeField] RectTransform CrossHair, CrossHairScope;
    [SerializeField] Transform lookTransform;
    public CinemachineVirtualCamera virtualCamera,ScopeCinemachine;
    [SerializeField] Camera ScopeView;
    [SerializeField] Button Aimbutton;

    public PickerWheel wheel;

    public Transform[] RedCribs, BlueCribs;
    public Transform GetScopeView { get { return ScopeView.transform; } }

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
       
        //SpinTheWheel();
    }

    private void OnspinComplete(WheelPiece obj)
    {
        PlayerPrefs.SetInt("WeaponIndex", obj.Amount);
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
        thirdPersonController.Context.setcamera(virtualCamera,ScopeView);
        thirdPersonController.Context.CrossHair .CrossHair= CrossHair;
        thirdPersonController.Context.WeaponIK.LookAt = lookTransform;
        Aimbutton.onClick.RemoveAllListeners();
        Aimbutton.onClick.AddListener(() => { thirdPersonController.SetScope(); });
    }


    public void SpawnPlayer()
    {
        PlayerCreator.Instance.SpawnObject();
    }

    public void ChangeView(bool b)
    {
        //ScopeView.gameObject.SetActive(b);
        //virtualCamera.gameObject.SetActive(!b);
        ScopeCinemachine.gameObject.SetActive(b);
        SniperUI.SetActive(b);
    }

    internal void SetScopeCamFeildView(int feildView)
    {
        ScopeView.fieldOfView = feildView;
        ScopeCinemachine.m_Lens.FieldOfView = feildView;
    }

    internal void SpinTheWheel()
    {
        wheel.transform.parent.gameObject.SetActive(true);
        wheel.Spin();
        wheel.OnSpinEnd(wheelPiece => { OnspinComplete(wheelPiece); });
    }
}
