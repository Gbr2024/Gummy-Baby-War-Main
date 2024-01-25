using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetWorkController : NetworkManager
{
    public static NetWorkController Instance;
    [SerializeField] NetworkManager networkManager;

    public bool isHost = false;
    private void Awake()
    {
        if(Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }

    }
    // Start is called before the first frame update
     void Start()
    {
       
    }

   


}
