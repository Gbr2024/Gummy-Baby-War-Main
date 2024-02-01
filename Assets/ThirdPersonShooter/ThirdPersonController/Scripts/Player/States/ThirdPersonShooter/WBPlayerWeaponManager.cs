using UnityEngine;

namespace WeirdBrothers.ThirdPersonController
{
    public struct WBPlayerWeaponManager : IState
    {
        private WBPlayerContext _context;
        private int _index;
        private RaycastHit _hit;
        private Vector3 _aimPoint;

        public WBPlayerWeaponManager(WBPlayerContext context)
        {
            _context = context;
            _index = 0;
            _hit = new RaycastHit();
            _aimPoint = Vector3.zero;
        }

        public void Execute()
        {
            if (_context.CurrentWeapon == null)
                return;

            if (Physics.Raycast(_context.PlayerCamera.transform.position,
                                _context.PlayerCamera.transform.forward,
                                out _hit,
                                 Mathf.Infinity,
                                 _context.Data.DamageLayer))
            {
                _aimPoint = _hit.point;
            }
            else
                _aimPoint = Vector3.zero;

            if (_context.Animator.IsSwitching())
                return;
            if (_context.Animator.IsReloading())
                return;
            if (_context.Animator.IsMeleeAttacking())
                return;

            if (_context.Input.GetButtonDown(WBInputKeys.Reload))
            {
                if (_context.CurrentWeapon.Data.FireType == FireType.None)
                    return;
                if (_context.CurrentWeapon.CurrentAmmo == _context.CurrentWeapon.Data.MagSize)
                    return;
                int totalAmmo = _context.Inventory.GetAmmo(_context.CurrentWeapon.Data.AmmoType);
                if (totalAmmo <= 0)
                    return;
                _context.Animator.OnReload();
            }
            else if (_context.CurrentWeapon.Data.FireType == FireType.Auto)
            {
                if (_context.Input.GetButton(WBInputKeys.Fire))
                {
                    OnFire(_hit.point);
                }
            }
            else if (_context.CurrentWeapon.Data.FireType == FireType.Semi)
            {
                if (_context.Input.GetButtonDown(WBInputKeys.Fire))
                {
                    OnFire(_hit.point);
                }
            }
            else if (_context.CurrentWeapon.Data.FireType == FireType.None)
            {
                if (_context.Input.GetButtonDown(WBInputKeys.Fire))
                {
                    _context.Animator.OnMeleeAttack();
                }
            }
        }

        private void OnFire(Vector3 hitPoint)
        {
            //_context.CurrentWeapon.Fire(hitPoint, _context.Data.DamageLayer);
            
            _context.ShooterController.FireInAll(hitPoint, _context.Data.DamageLayer);
            //_context.CurrentWeapon.FireBullet(hitPoint, layertoDamage);
            if (_context.CurrentWeapon.CurrentAmmo > 0)
            {
                _context.CrossHair.CrossHairSpread += _context.CurrentWeapon.Data.CrossHairSpread;
                _context.GenerateRecoil(_context.CurrentWeapon.Data.RecoilDuration);
            }
            if (_context.CurrentWeapon.Data.WeaponType == WBWeaponType.Primary)
            {
                if (_context.CurrentWeapon.WeaponSlot == WeaponSlot.First)
                {
                    _index = 1;
                }
                else
                {
                    _index = 2;
                }
            }
            else if (_context.CurrentWeapon.Data.WeaponType == WBWeaponType.Secondary)
            {
                _index = 3;
            }
            else if (_context.CurrentWeapon.Data.WeaponType == WBWeaponType.Melee)
            {
                _index = 4;
            }
            WBUIActions.SetPrimaryWeaponUI?.Invoke(_index, _context.CurrentWeapon.Data.WeaponImage,
             _context.CurrentWeapon.CurrentAmmo,
            _context.Inventory.GetAmmo(_context.CurrentWeapon.Data.AmmoType));
        }
    }
}