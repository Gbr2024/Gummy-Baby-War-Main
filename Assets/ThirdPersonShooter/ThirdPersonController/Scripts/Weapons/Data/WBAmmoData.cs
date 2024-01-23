using UnityEngine;

namespace WeirdBrothers.ThirdPersonController
{
    [CreateAssetMenu(fileName = "AmmoData", menuName = "WeirdBrothers/AmmoData")]
    public class WBAmmoData : ScriptableObject
    {
        public string AmmoType;
        public Sprite AmmoImage;
    }
}