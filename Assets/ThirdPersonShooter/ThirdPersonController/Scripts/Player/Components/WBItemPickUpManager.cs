using UnityEngine;

namespace WeirdBrothers.ThirdPersonController
{
    public struct WBItemPickUpManager
    {
        public void OnItemPickUp(WBPlayerContext context)
        {
            if (context.CurrentPickUpItem.CompareTag("Weapon"))
            {
                context.WeaponHandler.OnWeaponPickUP(context);
            }
            else if (context.CurrentPickUpItem.CompareTag("Ammo"))
            {
                WBPickUpAmmo pickUpAmmo = context.CurrentPickUpItem.GetComponent<WBPickUpAmmo>();
                if (pickUpAmmo)
                {
                    context.Inventory.AddItem(new WBItem
                    {
                        ItemName = pickUpAmmo.GetItemName(),
                        ItemType = WBItemType.Bullet,
                        ItemAmount = pickUpAmmo.Ammount
                    });
                }
                context.UpdateAmmo();
                GameObject.Destroy(context.CurrentPickUpItem.gameObject);
                WBUIActions.ShowItemPickUp(false, null, null);
            }
        }
    }
}