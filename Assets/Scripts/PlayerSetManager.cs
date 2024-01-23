using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using WeirdBrothers;
using WeirdBrothers.ThirdPersonController;
using Unity.Netcode;

public class PlayerSetManager : MonoBehaviour
{
    public static PlayerSetManager instance;
    [SerializeField] GameObject InputControl;
    [SerializeField] RectTransform CrossHair;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    public Transform[] RedCribs, BlueCribs;

    private void Awake()
    {
        instance = this;
        //Debug.LogError("instance Set");
    }

    // Start is called before the first frame update
    void Start()
    {

        PlayerCreator.Instance.SpawnObject();
    }

    public void setPlayerControlandCam(GameObject go)
    {
        go.GetComponent<WBThirdPersonController>().Context.CrossHair.CrossHair = CrossHair;
        go.GetComponent<WBThirdPersonController>().Context.setcamera(virtualCamera);
        virtualCamera.LookAt = go.transform;
        virtualCamera.Follow = go.transform;
        go.GetComponent<WBInputHandler>().SetInput(InputControl);
        Debug.LogError("data Set");
    }
}
