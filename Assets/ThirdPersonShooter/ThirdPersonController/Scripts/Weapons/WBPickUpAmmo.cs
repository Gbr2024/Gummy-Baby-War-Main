using UnityEngine;

namespace WeirdBrothers.ThirdPersonController
{
    public class WBPickUpAmmo : MonoBehaviour, IItemImage, IItemName
    {
        [SerializeField] private WBAmmoData _data;

        [SerializeField] private int _ammount;
        public int Ammount => _ammount;
        
        public Sprite GetItemImage()
        {
            return _data.AmmoImage;
        }

        public string GetItemName()
        {
            return _data.AmmoType;
        }
    }
}
