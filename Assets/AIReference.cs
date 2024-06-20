using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIReference : MonoBehaviour
{
    public static AIReference Instance;
    public Transform[] Points;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update

}
