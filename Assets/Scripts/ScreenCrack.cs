using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCrack : MonoBehaviour
{
    [SerializeField] GameObject[] childrens;
    [SerializeField] AudioSource audio;

    private void OnEnable()
    {
        foreach (var item in childrens)
        {
            item.SetActive(Random.Range(0, 2) == 1);
        }
        audio.Play();
        Invoke(nameof(DisableScreen), 6f);
    }


    void DisableScreen()
    {
        gameObject.SetActive(false);
    }
}
