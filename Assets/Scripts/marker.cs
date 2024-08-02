using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class marker : MonoBehaviour
{
    [SerializeField] GameObject Body;

    internal void EnableBody(bool activate,bool SetDiActive=true)
    {
        Body.SetActive(activate);
        if(SetDiActive) StartCoroutine(DisableBody());
    }

    IEnumerator DisableBody()
    {
        yield return new WaitForSeconds(7f);
        Body.SetActive(false);
    }

    internal void SetColor(Color color)
    {
        var mats = GetComponentsInChildren<MeshRenderer>(transform);
        foreach (var item in mats)
        {
            item.material.SetColor("_BaseColor", color);
        }

    }
}
