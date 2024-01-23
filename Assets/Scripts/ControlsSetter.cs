using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ControlsSetter : NetworkBehaviour
{

    private void Start()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            PlayerSetManager.instance.setPlayerControlandCam(gameObject);
            if(CustomProperties.Instance.isRed)
            {
                transform.position = PlayerSetManager.instance.RedCribs[Random.Range(0,3)].position;
            }
            else
            {
                transform.position = PlayerSetManager.instance.BlueCribs[Random.Range(0, 3)].position;
            }
        }
    }
}
