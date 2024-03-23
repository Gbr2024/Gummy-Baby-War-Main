using System;
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
            if (_context.Input.GetButtonDown(WBInputKeys.GrenadeEnable))
            {
                OnSwitchToGrenade();
            }
            if (_context.CurrentWeapon == null)
                return;

            var Cam = _context.PlayerCamera.transform;//_context.isScopeOn ? _context.PlayerScopeCamera.transform : _context.PlayerCamera.transform;
            if (Physics.Raycast(Cam.position,
                                Cam.forward,
                                out _hit,
                                 Mathf.Infinity,
                                 _context.Data.DamageLayer))
            {

                _aimPoint = _hit.point;
                //_context.CurrentWeapon.transform.LookAt(_aimPoint);

            }
            else
                _aimPoint = Cam.GetChild(0).position; //Cam.forward*_context.CurrentWeapon.Data.Range;

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
                if (_context.isScopeOn) _context.ShooterController.SetScope();
                _context.Animator.OnReload();
            }
            if (_context.Input.GetButtonUp(WBInputKeys.Fire))
            {
                _context.isAiming = false;
                _context.Animator.SetAim(false); 
            }
            
            if (_context.Input.GetButtonUp(WBInputKeys.KillStreak))
            {
                KillStreakSystem.Instance.SetKillstreak(_context.ShooterController.OwnerClientId, CustomProperties.Instance.isRed, PlayerCreator.Instance.killstreak);
            }


            else if (_context.CurrentWeapon.Data.FireType == FireType.Auto)
            {
                if (_context.Input.GetButton(WBInputKeys.Fire))
                {
                    OnFire(_aimPoint);
                }
            }
            else if (_context.CurrentWeapon.Data.FireType == FireType.Semi)
            {
                if (_context.Input.GetButtonDown(WBInputKeys.Fire))
                {
                    OnFire(_aimPoint);
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


        private void OnSwitchToGrenade()
        {

            if(_context.ShooterController.IsOwner)
            {
                if(!_context.GrenadeSet)
                {
                    _context.trajectory.EnableDraw(true);
                    _context.ShooterController.syncer.isWeaponActivated.Value = false;
                    _context.GrenadeSet = true;
                    _context.ShooterController.CreateGrenade();
                    WBUIActions.EnableGrenadeTime?.Invoke(true);
                    WBUIActions.ChangeFireIcon?.Invoke("Grenade");
                    _context.Animator._animator.SetBool("GrenadeThrow", false);
                    _context.Animator._animator.SetBool("ThrowCancel", false);
                    _context.Animator._animator.SetTrigger("GrenadeStart");
                    _context.CurrentWeapon.Body.SetActive(false);
                }
                else
                {
                    ResetGrenade();
                    if (_context.trajectory.grenade != null) _context.ShooterController.DespawnGrenade();
                    WBUIActions.EnableGrenadeTime?.Invoke(false);
                    WBUIActions.ChangeFireIcon?.Invoke("Gun");
                }
                
            }
        }

        void ResetGrenade()
        {
            _context.trajectory.EnableDraw(false);
            _context.Animator._animator.SetBool("ThrowCancel", true);
            _context.ShooterController.ActivateBodyOnWait();
            _context.GrenadeSet = false;

        }
        

        private void OnFire(Vector3 hitPoint)
        {
            //_context.CurrentWeapon.Fire(hitPoint, _context.Data.DamageLayer);
            //_context.CurrentWeapon.transform.LookAt(_aimPoint);
            //Debug.LogError(hitPoint);
            _context.Animator.SetAim(true);
            _context.isAiming = true;
            if (_context.GrenadeSet)
            {
                _context.Animator._animator.SetBool("GrenadeThrow",true);
                _context.trajectory.grenade.ToFollow = null;
                _context.trajectory.ThrowObject();
                WBUIActions.ChangeFireIcon?.Invoke("Gun");
                ResetGrenade();
                _context.GrenadeCount--;
                if(_context.GrenadeCount<=0)
                    WBUIActions.EnableGrenadeButton?.Invoke(false);
                return;
            }

            if (!_context.CurrentWeapon.Body.activeSelf) return;

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
            if (_context.ShooterController.IsOwner)
                WBUIActions.SetPrimaryWeaponUI?.Invoke(_index, _context.CurrentWeapon.Data.WeaponImage,
             _context.CurrentWeapon.CurrentAmmo,
            _context.Inventory.GetAmmo(_context.CurrentWeapon.Data.AmmoType));
        }

        internal void SetRotationLookForCurrentWeapon()
        {
            _context.CurrentWeapon.transform.LookAt(_aimPoint);
        }
    }
}