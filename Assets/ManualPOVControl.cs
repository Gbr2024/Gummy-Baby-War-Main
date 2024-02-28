using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ManualPOVControl : CinemachineBrain
{
    public CinemachineVirtualCameraBase vcam;
    private void Update()
    {
        if(!vcam.gameObject.activeSelf)
            vcam.UpdateCameraState(DefaultWorldUp, Time.fixedDeltaTime);
    }
}
