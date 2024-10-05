using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    public static CloudManager instance;

    [SerializeField] Material CloudMat;
    [SerializeField] Color MainColor;
    [SerializeField] RainManager rainManager;
    
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        ChangeColorAndIntensity();
    }
    void Start()
    {
        
        if(LobbyManager.Instance.GetStorm()==1) EnableRain();
    }

    void ChangeColorAndIntensity()
    {
        // Modify the color
        CloudMat.SetColor("_BaseColor", MainColor);  // Adjust brightness by intensity

        // Alternatively, if you're using a shader with emission (to handle intensity):
        CloudMat.EnableKeyword("_EMISSION");

    }

    internal void EnableRain()
    {
        rainManager.gameObject.SetActive(true);
    }

}
