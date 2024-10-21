using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class internetChecker : MonoBehaviour
{
    [SerializeField] GameObject Body;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable && !Body.activeSelf)
        {
            Body.SetActive(true);
        }
        else if((Application.internetReachability != NetworkReachability.NotReachable) && Body.activeSelf)
        {
            Body.SetActive(false);
        }
    }
}
