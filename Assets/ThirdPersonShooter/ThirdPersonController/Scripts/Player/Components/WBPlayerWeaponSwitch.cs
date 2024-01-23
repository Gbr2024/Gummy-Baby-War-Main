using UnityEngine;

namespace WeirdBrothers.ThirdPersonController
{
    public struct WBPlayerWeaponSwitch : IState
    {
        private WBPlayerContext _context;

        public WBPlayerWeaponSwitch(WBPlayerContext context)
        {
            _context = context;
        }

        public void Execute()
        {
            if (_context.Animator.IsSwitching())
                return;
            if (_context.Animator.IsReloading())
                return;
            if (_context.Animator.IsMeleeAttacking())
                return;

            if (_context.Input.GetButtonDown(WBInputKeys.Switch1))
            {
                if (_context.CurrentWeapon == null &&
                    _context.WeaponSlots.PrimarySlot1.GetActiveChildTransform() != null)
                {
                    _context.Animator.WeaponSwitch(1);
                }
                else if (_context.CurrentWeapon != null)
                {
                    if (_context.CurrentWeapon.Data.WeaponType == WBWeaponType.Primary)
                    {
                        if (_context.CurrentWeapon.WeaponSlot == WeaponSlot.First)
                        {
                            _context.Animator.WeaponSwitch(1);
                        }
                        else if (_context.CurrentWeapon.WeaponSlot == WeaponSlot.Second &&
                                    _context.WeaponSlots.PrimarySlot1.GetActiveChildTransform() != null)
                        {
                            _context.Animator.WeaponSwitch(4);
                        }
                    }
                    else if (_context.CurrentWeapon.Data.WeaponType == WBWeaponType.Secondary &&
                                _context.WeaponSlots.PrimarySlot1.GetActiveChildTransform() != null)
                    {
                        _context.Animator.WeaponSwitch(8);
                    }
                    else if (_context.CurrentWeapon.Data.WeaponType == WBWeaponType.Melee &&
                                _context.WeaponSlots.PrimarySlot1.GetActiveChildTransform() != null)
                    {
                        _context.Animator.WeaponSwitch(14);
                    }
                }

            }
            else if (_context.Input.GetButtonDown(WBInputKeys.Switch2))
            {
                if (_context.CurrentWeapon == null &&
                    _context.WeaponSlots.PrimarySlot2.GetActiveChildTransform() != null)
                {
                    _context.Animator.WeaponSwitch(2);
                }
                else if (_context.CurrentWeapon != null)
                {
                    if (_context.CurrentWeapon.Data.WeaponType == WBWeaponType.Primary)
                    {
                        if (_context.CurrentWeapon.WeaponSlot == WeaponSlot.Second)
                        {
                            _context.Animator.WeaponSwitch(2);
                        }
                        else if (_context.CurrentWeapon.WeaponSlot == WeaponSlot.First &&
                                    _context.WeaponSlots.PrimarySlot2.GetActiveChildTransform() != null)
                        {
                            _context.Animator.WeaponSwitch(3);
                        }
                    }
                    else if (_context.CurrentWeapon.Data.WeaponType == WBWeaponType.Secondary &&
                                _context.WeaponSlots.PrimarySlot2.GetActiveChildTransform() != null)
                    {
                        _context.Animator.WeaponSwitch(9);
                    }
                    else if (_context.CurrentWeapon.Data.WeaponType == WBWeaponType.Melee &&
                                _context.WeaponSlots.PrimarySlot2.GetActiveChildTransform() != null)
                    {
                        _context.Animator.WeaponSwitch(15);
                    }
                }
            }
            else if (_context.Input.GetButtonDown(WBInputKeys.Switch3))
            {
                if (_context.CurrentWeapon == null &&
                    _context.WeaponSlots.SecondarySlot.GetActiveChildTransform() != null)
                {
                    _context.Animator.WeaponSwitch(5);
                }
                else if (_context.CurrentWeapon != null)
                {
                    if (_context.CurrentWeapon.Data.WeaponType == WBWeaponType.Secondary)
                    {
                        _context.Animator.WeaponSwitch(5);
                    }
                    else if (_context.CurrentWeapon.Data.WeaponType == WBWeaponType.Melee &&
                                _context.WeaponSlots.SecondarySlot.GetActiveChildTransform() != null)
                    {
                        _context.Animator.WeaponSwitch(16);
                    }
                    else if (_context.CurrentWeapon.Data.WeaponType == WBWeaponType.Primary)
                    {
                        if (_context.CurrentWeapon.WeaponSlot == WeaponSlot.First &&
                                _context.WeaponSlots.SecondarySlot.GetActiveChildTransform() != null)
                        {
                            _context.Animator.WeaponSwitch(6);
                        }
                        else if (_context.CurrentWeapon.WeaponSlot == WeaponSlot.Second &&
                                    _context.WeaponSlots.SecondarySlot.GetActiveChildTransform() != null)
                        {
                            _context.Animator.WeaponSwitch(7);
                        }
                    }
                }
            }
            else if (_context.Input.GetButtonDown(WBInputKeys.Switch4))
            {
                if (_context.CurrentWeapon == null &&
                    _context.WeaponSlots.MeleeSlot.GetActiveChildTransform() != null)
                {
                    _context.Animator.WeaponSwitch(10);
                }
                else if (_context.CurrentWeapon != null)
                {
                    if (_context.CurrentWeapon.Data.WeaponType == WBWeaponType.Melee)
                    {
                        _context.Animator.WeaponSwitch(10);
                    }
                    else if (_context.CurrentWeapon.Data.WeaponType == WBWeaponType.Secondary &&
                    _context.WeaponSlots.MeleeSlot.GetActiveChildTransform() != null)
                    {
                        _context.Animator.WeaponSwitch(11);
                    }
                    else if (_context.CurrentWeapon.Data.WeaponType == WBWeaponType.Primary)
                    {
                        if (_context.CurrentWeapon.WeaponSlot == WeaponSlot.First &&
                            _context.WeaponSlots.MeleeSlot.GetActiveChildTransform() != null)
                        {
                            _context.Animator.WeaponSwitch(12);
                        }
                        else if (_context.CurrentWeapon.WeaponSlot == WeaponSlot.Second &&
                                _context.WeaponSlots.MeleeSlot.GetActiveChildTransform() != null)
                        {
                            _context.Animator.WeaponSwitch(13);
                        }
                    }
                }
            }
        }
    }
}