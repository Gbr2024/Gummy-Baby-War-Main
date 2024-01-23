using System;
using UnityEngine;

namespace WeirdBrothers.IKHepler 
{
    [System.Serializable]
    public class LookAtIK
    {
        public Transform LookAt;
        public Transform Spine;
        public Vector3 SpineRotation;
    }
}
