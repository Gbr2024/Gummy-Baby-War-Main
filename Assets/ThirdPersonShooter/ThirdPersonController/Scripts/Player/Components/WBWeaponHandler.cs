using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

namespace WeirdBrothers.ThirdPersonController
{
    public class WBWeaponHandler
    {
        public void OnWeaponPickUP(WBPlayerContext context)
        {
            WBWeapon currentPickUpWeapon = context.CurrentPickUpItem.GetComponent<WBWeapon>();
            WBWeaponData data = currentPickUpWeapon.Data;
            WBWeapon[] weapons = context.Transform.GetComponentsInChildren<WBWeapon>();
           // Debug.LogError("OnWeapon SetUp");
            if (data.WeaponType == WBWeaponType.Primary)
            {
                List<WBWeapon> primarayWeapons = new List<WBWeapon>();
                Array.ForEach(weapons, weapon =>
                {
                    if (weapon.Data.WeaponType == WBWeaponType.Primary)
                    {
                        primarayWeapons.Add(weapon);
                    }
                });

                if (primarayWeapons.Count <= 0 && context.CurrentWeapon == null)
                {
                    EquipWeapon(currentPickUpWeapon, data.WeaponHandPosition,
                                context.WeaponSlots.RightHandReference,
                                WeaponSlot.First,
                                context);
                }
                else
                {
                    if (primarayWeapons.Count == 0)
                    {
                        EquipWeapon(currentPickUpWeapon, data.WeaponSlotPosition,
                                    context.WeaponSlots.PrimarySlot1,
                                    WeaponSlot.First,
                                    context);
                    }
                    else if (primarayWeapons.Count == 1)
                    {
                        if (primarayWeapons[0].Data.WeaponType == WBWeaponType.Primary)
                        {
                            if (context.CurrentWeapon == null)
                            {
                                EquipWeapon(currentPickUpWeapon, data.WeaponHandPosition,
                                            context.WeaponSlots.RightHandReference,
                                            WeaponSlot.Second,
                                            context);
                            }
                            else
                            {
                                EquipWeapon(currentPickUpWeapon, data.WeaponSlot2Position,
                                            context.WeaponSlots.PrimarySlot2,
                                            WeaponSlot.Second,
                                            context);
                            }
                        }
                    }
                    else
                    {
                        if (context.CurrentWeapon == null)
                        {
                            DropWeapon(context, context.WeaponSlots.PrimarySlot1.GetActiveChildTransform());
                            EquipWeapon(currentPickUpWeapon, data.WeaponSlotPosition,
                                        context.WeaponSlots.PrimarySlot1,
                                        WeaponSlot.First,
                                        context);
                        }
                        else
                        {
                            if (context.CurrentWeapon.WeaponSlot == WeaponSlot.First)
                            {
                                DropWeapon(context, context.WeaponSlots.RightHandReference.GetActiveChildTransform());
                                EquipWeapon(currentPickUpWeapon, data.WeaponHandPosition,
                                            context.WeaponSlots.RightHandReference,
                                            WeaponSlot.First,
                                            context);
                            }
                            else
                            {
                                DropWeapon(context, context.WeaponSlots.RightHandReference.GetActiveChildTransform());
                                EquipWeapon(currentPickUpWeapon, data.WeaponHandPosition,
                                            context.WeaponSlots.RightHandReference,
                                            WeaponSlot.Second,
                                            context);
                                context.CurrentWeapon = currentPickUpWeapon;
                            }
                        }
                    }
                }
            }
            else if (data.WeaponType == WBWeaponType.Secondary)
            {
                List<WBWeapon> secondaryWeapons = new List<WBWeapon>();
                Array.ForEach(weapons, weapon =>
                {
                    if (weapon.Data.WeaponType == WBWeaponType.Secondary)
                    {
                        secondaryWeapons.Add(weapon);
                    }
                });

                if (secondaryWeapons.Count == 1)
                {
                    if (context.CurrentWeapon &&
                    context.WeaponSlots.SecondarySlot.GetActiveChildTransform() != null)
                    {
                        DropWeapon(context, context.WeaponSlots.SecondarySlot);
                        EquipWeapon(currentPickUpWeapon, data.WeaponSlotPosition,
                                    context.WeaponSlots.SecondarySlot,
                                    WeaponSlot.None,
                                    context);
                    }
                    else if (context.CurrentWeapon.Data.WeaponType == WBWeaponType.Secondary)
                    {
                        DropWeapon(context, context.CurrentWeapon.transform);
                        EquipWeapon(currentPickUpWeapon, data.WeaponHandPosition,
                                    context.WeaponSlots.RightHandReference,
                                    WeaponSlot.None,
                                    context);
                    }
                }
                else
                {
                    if (context.CurrentWeapon == null)
                    {
                        EquipWeapon(currentPickUpWeapon, data.WeaponHandPosition,
                                    context.WeaponSlots.RightHandReference,
                                    WeaponSlot.None,
                                    context);
                    }
                    else
                    {
                        EquipWeapon(currentPickUpWeapon, data.WeaponSlotPosition,
                                    context.WeaponSlots.SecondarySlot,
                                    WeaponSlot.None,
                                    context);
                    }
                }
            }
            else if (data.WeaponType == WBWeaponType.Melee)
            {
                List<WBWeapon> meleeWeapons = new List<WBWeapon>();
                Array.ForEach(weapons, weapon =>
                {
                    if (weapon.Data.WeaponType == WBWeaponType.Melee)
                    {
                        meleeWeapons.Add(weapon);
                    }
                });

                if (meleeWeapons.Count == 1)
                {
                    if (context.CurrentWeapon &&
                    context.WeaponSlots.MeleeSlot.GetActiveChildTransform() != null)
                    {
                        DropWeapon(context, context.WeaponSlots.MeleeSlot);
                        EquipWeapon(currentPickUpWeapon, data.WeaponSlotPosition,
                                    context.WeaponSlots.MeleeSlot,
                                    WeaponSlot.None,
                                    context);
                    }
                    else if (context.CurrentWeapon.Data.WeaponType == WBWeaponType.Melee)
                    {
                        DropWeapon(context, context.CurrentWeapon.transform);
                        EquipWeapon(currentPickUpWeapon, data.WeaponHandPosition,
                                    context.WeaponSlots.RightHandReference,
                                    WeaponSlot.None,
                                    context);
                    }
                }
                else
                {
                    if (context.CurrentWeapon == null)
                    {
                        EquipWeapon(currentPickUpWeapon, data.WeaponHandPosition,
                                    context.WeaponSlots.RightHandReference,
                                    WeaponSlot.None,
                                    context);
                    }
                    else
                    {
                        EquipWeapon(currentPickUpWeapon, data.WeaponSlotPosition,
                                    context.WeaponSlots.MeleeSlot,
                                    WeaponSlot.None,
                                    context);
                    }
                }
            }
        }

        private void EquipWeapon(WBWeapon currentPickUpWeapon, WBWeaponPositionData data, Transform parent, WeaponSlot slot, WBPlayerContext context)
        {
            Debug.LogError("On Equip");
            currentPickUpWeapon.transform.SetParent(parent);
            //currentPickUpWeapon.RemoveRigidBody();
            currentPickUpWeapon.WeaponSlot = slot;
            currentPickUpWeapon.gameObject.layer = LayerMask.NameToLayer("Weapon");
            if(context.CurrentWeapon==null)context.CurrentWeapon = currentPickUpWeapon;
            currentPickUpWeapon.transform.localPosition = data.Position;
            currentPickUpWeapon.transform.localRotation = Quaternion.Euler(data.Rotation);
            context.CurrentPickUpItem = null;
            //if(context.ShooterController.IsLocalPlayer)
            WBUIActions.ShowItemPickUp?.Invoke(false, null, "");

            var index = 1;
            if (currentPickUpWeapon.Data.WeaponType == WBWeaponType.Primary)
            {
                if (slot == WeaponSlot.First)
                {
                    index = 1;
                }
                else if (slot == WeaponSlot.Second)
                {
                    index = 2;
                }
            }
            else if (currentPickUpWeapon.Data.WeaponType == WBWeaponType.Secondary)
            {
                index = 3;
            }
            else if (currentPickUpWeapon.Data.WeaponType == WBWeaponType.Melee)
            {
                index = 4;
            }

            var weaponImage = currentPickUpWeapon.gameObject.GetItemImage();
            var currentAmmo = currentPickUpWeapon.CurrentAmmo;
            var totalAmmo = context.Inventory.GetAmmo(currentPickUpWeapon.Data.AmmoType);
            WBUIActions.SetPrimaryWeaponUI?.Invoke(index, weaponImage, currentAmmo, totalAmmo);


            // if (context.ShooterController.IsLocalPlayer)

            //Debug.LogError(context.Animator);
            //Debug.LogError(currentPickUpWeapon.Data.WeaponIndex);
            if (parent.GetInstanceID() == context.WeaponSlots.RightHandReference.GetInstanceID())
            {
                Debug.LogError("Here");
                context.SetAnimator(currentPickUpWeapon.Data.WeaponIndex);
            }

            //OnWeaponSwitch(context, 1);
        }

        private void DropWeapon(WBPlayerContext context, Transform weaponToDrop)
        {
            weaponToDrop.GetComponent<WBWeapon>().WeaponSlot = WeaponSlot.None;
            weaponToDrop.SetParent(null);
            weaponToDrop.gameObject.layer = LayerMask.NameToLayer("ItemPickUp");
            weaponToDrop.gameObject.AddComponent<Rigidbody>().AddForce(context.Transform.up * 100 * Time.deltaTime, ForceMode.Impulse);
        }

        public void OnWeaponSwitch(WBPlayerContext context, int index)
        {
           
            var currentWeapon = GetCurrentWeapon(context);
            if (currentWeapon != null)
            {
                Debug.LogError("1");
                if (index == 1 && currentWeapon.WeaponSlot == WeaponSlot.First)
                {
                    Debug.LogError("1.1");
                    Debug.LogError(currentWeapon);
                    Debug.LogError(currentWeapon.Data.WeaponSlotPosition);
                    Debug.LogError(context.WeaponSlots.PrimarySlot1);
                    Debug.LogError(currentWeapon.WeaponSlot);
                    Debug.LogError(context);

                    EquipWeapon(currentWeapon, currentWeapon.Data.WeaponSlotPosition,
                                context.WeaponSlots.PrimarySlot1, currentWeapon.WeaponSlot, context);
                    Debug.LogError("1.1-");
                }
                else if (index == 2 && currentWeapon.WeaponSlot == WeaponSlot.Second)
                {
                    Debug.LogError("1.2");
                    EquipWeapon(currentWeapon, currentWeapon.Data.WeaponSlot2Position,
                                context.WeaponSlots.PrimarySlot2, currentWeapon.WeaponSlot, context);
                    Debug.LogError("1.2-");
                }
                else if (index == 3 && currentWeapon.Data.WeaponType == WBWeaponType.Secondary)
                {
                    Debug.LogError("1.3");
                    EquipWeapon(currentWeapon, currentWeapon.Data.WeaponSlotPosition,
                                context.WeaponSlots.SecondarySlot, currentWeapon.WeaponSlot, context);
                }
                else if (index == 4 && currentWeapon.Data.WeaponType == WBWeaponType.Melee)
                {
                    Debug.LogError("1.4");
                    EquipWeapon(currentWeapon, currentWeapon.Data.WeaponSlotPosition,
                                context.WeaponSlots.MeleeSlot, currentWeapon.WeaponSlot, context);
                }
            }
            else
            {
                Debug.LogError("2");
                if (index == 1 &&
                    context.WeaponSlots.PrimarySlot1.GetActiveChildTransform() != null)
                {
                    var weapon = context.WeaponSlots.PrimarySlot1.GetActiveChildTransform().GetComponent<WBWeapon>();
                    EquipWeapon(weapon, weapon.Data.WeaponHandPosition,
                                context.WeaponSlots.RightHandReference, weapon.WeaponSlot, context);
                }
                else if (index == 2 &&
                    context.WeaponSlots.PrimarySlot2.GetActiveChildTransform() != null)
                {
                    var weapon = context.WeaponSlots.PrimarySlot2.GetActiveChildTransform().GetComponent<WBWeapon>();
                    EquipWeapon(weapon, weapon.Data.WeaponHandPosition,
                                context.WeaponSlots.RightHandReference, weapon.WeaponSlot, context);
                }
                else if (index == 3 &&
                    context.WeaponSlots.SecondarySlot.GetActiveChildTransform() != null)
                {
                    var weapon = context.WeaponSlots.SecondarySlot.GetActiveChildTransform().GetComponent<WBWeapon>();
                    EquipWeapon(weapon, weapon.Data.WeaponHandPosition,
                                context.WeaponSlots.RightHandReference, weapon.WeaponSlot, context);
                }
                else if (index == 4 &&
                    context.WeaponSlots.MeleeSlot.GetActiveChildTransform() != null)
                {
                    var weapon = context.WeaponSlots.MeleeSlot.GetActiveChildTransform().GetComponent<WBWeapon>();
                    EquipWeapon(weapon, weapon.Data.WeaponHandPosition,
                                context.WeaponSlots.RightHandReference, weapon.WeaponSlot, context);
                }
            }
        }

        public WBWeapon GetCurrentWeapon(WBPlayerContext context)
        {
            var weaponSlot = context.WeaponSlots.RightHandReference.GetActiveChildTransform();
            if (weaponSlot != null)
            {
                WBUIActions.SetWeaponUI?.Invoke(true);
                if (weaponSlot.GetComponent<WBWeapon>().Data.WeaponType == WBWeaponType.Melee)
                {
                    context.CrossHair.CrossHair.gameObject.SetActive(false);
                }
                else
                    context.CrossHair.CrossHair.gameObject.SetActive(true);
                return weaponSlot.GetComponent<WBWeapon>();
            }
            context.CrossHair.CrossHair.gameObject.SetActive(false);
            WBUIActions.SetWeaponUI?.Invoke(false);
            return null;
        }

        public void OnMagIn(WBPlayerContext context)
        {
            int bulletsToLoad = context.CurrentWeapon.Data.MagSize - context.CurrentWeapon.CurrentAmmo;
            int totalAmmo = context.Inventory.GetAmmo(context.CurrentWeapon.Data.AmmoType);
            int bulletsToDeduct = totalAmmo >= bulletsToLoad ? bulletsToLoad : totalAmmo;

            context.CurrentWeapon.AddAmmo(bulletsToDeduct);
            //totalAmmo -= bulletsToDeduct;
            context.Inventory.UpdateItem(new WBItem
            {
                ItemName = context.CurrentWeapon.Data.AmmoType,
                ItemType = WBItemType.Bullet,
                ItemAmount = totalAmmo
            });
            context.UpdateAmmo(context.CurrentWeapon);
        }
    }
}
