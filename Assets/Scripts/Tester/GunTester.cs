using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeirdBrothers.ThirdPersonController;

public class GunTester : MonoBehaviour
{
    [SerializeField] int Index;
    [SerializeField] WBWeapon[] weapons;



    // Start is called before the first frame update
    void Start()
    {

    }

    public void setfiring(bool b)
    {
        weapons[Index].setfiring(b);
    }

    public void setindex(int i)
    {
        weapons[Index].gameObject.SetActive(false);
        Index++;
        if (Index < 0 || Index >= weapons.Length)
            Index = 0;
        weapons[Index].gameObject.SetActive(true);
    }

}

   
