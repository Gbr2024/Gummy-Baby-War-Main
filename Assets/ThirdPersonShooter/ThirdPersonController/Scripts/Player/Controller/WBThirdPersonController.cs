using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using System.Collections;

namespace WeirdBrothers.ThirdPersonController
{
    public class WBThirdPersonController : NetworkBehaviour
    {
        [SerializeField] private WBPlayerContext _context;
        public WBPlayerContext Context => _context;

        [SerializeField] private PlayerState[] _states;

        private List<object> _playerStates;
        private WBPlayerMovement _movement;
        private WBPlayerIKHandle _ikHandler;

        bool isDataSet = false;

        public IEnumerator Start()
        {
            yield return new WaitForSeconds(.1f);
            if(IsOwner)
            {
                //Debug.LogError(_context.PlayerCamera);
                _context.SetData(transform);
                _states = _states.Distinct().ToArray();
                _playerStates = new List<object>();
                _movement = new WBPlayerMovement(_context);
                _ikHandler = new WBPlayerIKHandle(_context);

                WBPlayerGroundChecker groundChecker = new WBPlayerGroundChecker(_context);
                WBPlayerJump jump = new WBPlayerJump(_context);
                _playerStates.Add(groundChecker);
                _playerStates.Add(jump);

                if (ContainsShooter())
                {
                    WBPlayerItemPickUp itemPickUp = new WBPlayerItemPickUp(_context);
                    WBPlayerWeaponSwitch weaponSwitch = new WBPlayerWeaponSwitch(_context);
                    WBPlayerWeaponManager weaponManager = new WBPlayerWeaponManager(_context);

                    _playerStates.Add(itemPickUp);
                    _playerStates.Add(weaponSwitch);
                    _playerStates.Add(weaponManager);
                }
                isDataSet = true;
            }
        }

        private bool ContainsShooter()
        {
            foreach (var state in _states)
            {
                if (state == PlayerState.Shooter)
                    return true;
            }
            return false;
        }

        private void FixedUpdate()
        {
            if(IsOwner && isDataSet)
                _movement.Schedule();
        }

        private void Update()
        {
            if (IsOwner && isDataSet)
            {
                Array.ForEach(_playerStates.ToArray(), state =>
                {
                    state.Schedule();
                });
                _context.CurrentWeapon = _context.WeaponHandler.GetCurrentWeapon(_context);

                _context.CrossHair.CrossHairSpread = Mathf.Clamp(_context.CrossHair.CrossHairSpread, _context.CrossHair.MinSpread, _context.CrossHair.MaxSpread);
                _context.CrossHair.CrossHair.sizeDelta = new Vector2(_context.CrossHair.CrossHairSpread,
                    _context.CrossHair.CrossHairSpread);

                if (_context.RecoilTime > 0)
                {
                    _context.Pov.m_VerticalAxis.Value -= _context.CurrentWeapon.Data.VerticalRecoil;
                    _context.Pov.m_HorizontalAxis.Value -= UnityEngine.Random.Range(-_context.CurrentWeapon.Data.HorizontalRecoil,
                                                                _context.CurrentWeapon.Data.HorizontalRecoil);
                    _context.RecoilTime -= Time.deltaTime;
                }
            }
        }

        private void LateUpdate()
        {
            if(isDataSet && IsOwner)
                _ikHandler.Schedule();
        }

        private void OnTriggerStay(Collider other)
        {
            if(IsOwner)
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("ItemPickUp"))
                {
                    if (!ContainsShooter())
                        return;

                    _context.CurrentPickUpItem = other.transform;
                    var itemImage = other.gameObject.GetItemImage();
                    var itemName = other.gameObject.GetItemName();
                    WBUIActions.ShowItemPickUp?.Invoke(true, itemImage, itemName);
                }
            }
            
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsOwner)
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("ItemPickUp"))
                {
                    _context.CurrentPickUpItem = null;
                    WBUIActions.ShowItemPickUp?.Invoke(false, null, "");
                }
            }
        }

        private void OnItemEquip()
        {
            if (_context.CurrentPickUpItem != null)
            {
                _context.PickUpManager.OnItemPickUp(_context);
            }
        }

        private void OnWeaponSwitch(int index)
        {
            _context.WeaponHandler.OnWeaponSwitch(_context, index);
        }

        private void OnSwitchStart()
        {
            _context.Animator.OnSwitch(true);
        }

        private void OnSwitchEnd()
        {
            _context.Animator.OnSwitch(false);
            _context.SetAnimator();
        }

        private void OnMagIn()
        {
            _context.WeaponHandler.OnMagIn(_context);
            _context.CurrentWeapon.MagIn();
        }

        private void OnMagOut()
        {
            _context.CurrentWeapon.MagOut();
        }

        private void OnBolt()
        {
            _context.CurrentWeapon.Bolt();
        }

        private void OnMeleeAttack()
        {
            CheckForEnemies(_context.CurrentWeapon.transform);
        }

        private void CheckForEnemies(Transform obj)
        {
            Collider[] hittedEnemies = Physics.OverlapSphere(obj.position,
                                                            _context.CurrentWeapon.Data.Range,
                                                            _context.Data.DamageLayer);

            foreach (Collider col in hittedEnemies)
            {
                Debug.Log(col.transform.name);
                col.gameObject.ApplyDamage(_context.CurrentWeapon.Data.Damage, transform.position);
            }
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (!isDataSet) return;
            if (_context.Animator.IsReloading())
            {
                return;
            }

            if (_context.CurrentWeapon != null)
            {
                var handRef = _context.CurrentWeapon.LeftHandRef;
                if (handRef == null) return;

                _context.Animator.setLeftHand(handRef.position, handRef.rotation);
                return;
            }
        }
    }
}