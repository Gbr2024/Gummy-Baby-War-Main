using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using DG.Tweening;
using System.Collections;
using Random = UnityEngine.Random;

namespace WeirdBrothers.ThirdPersonController
{
    public class WBThirdPersonController : NetworkBehaviour
    {
        [SerializeField] private WBPlayerContext _context;
        public WBPlayerContext Context => _context;


        [SerializeField] private PlayerState[] _states;
        Syncer syncer;

        private List<object> _playerStates;
        private WBPlayerMovement _movement;
        private WBPlayerIKHandle _ikHandler;

        internal bool isRed = false;
        internal int bulletlayer;
        bool isDataSet = false;
        int mykills = 0;

        private void Awake()
        {
            syncer=GetComponent<Syncer>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            //if (!IsOwner)
            //    enabled = false;
            //Debug.LogError(_context.PlayerCamera);
            setContext();
            if(IsOwner)
            {
                syncer.isRed.Value = isRed;
                if (CustomProperties.Instance.isRed)
                    gameObject.layer = 10;
                else
                    gameObject.layer = 13;

                SetWeaponData(PlayerPrefs.GetInt("WeaponIndex"), bulletlayer);
                syncer.WeaponIndex.Value = PlayerPrefs.GetInt("WeaponIndex");
            }
            
        }

       

        internal void setContext()
        {
            PlayerSetManager.instance.SetCamera(this);
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
            if (IsOwner && isDataSet)
                _movement.Schedule();
        }

        [ServerRpc (RequireOwnership =false)]
        internal void SetColorServerRpc(ulong id, int colorIndex)
        {
            SetColorClientRpc(id, colorIndex);
        }

        [ClientRpc]
        private void SetColorClientRpc(ulong id,int colorIndex)
        {
            if (NetworkObject.OwnerClientId != id) return;
            
        }

        private void Update()
        {
            if (IsOwner && isDataSet && !Context.health.isDead)
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

        internal void FireInAll(Vector3 hitPoint, LayerMask damageLayer)
        {
            var camRot = Camera.main.transform.rotation;
           
            if(_context.CurrentWeapon.CurrentAmmo>0)
            {
                
                ShootServerRpc(NetworkObject.OwnerClientId, hitPoint, camRot,Context.CurrentWeapon.GetMuzzleFlah.position);
                _context.CurrentWeapon.FireBullet(hitPoint, camRot, Context.CurrentWeapon.GetMuzzleFlah.position);
            }
            
            
        }

        [ServerRpc (RequireOwnership =false)]
        void ShootServerRpc(ulong id, Vector3 hitPoint, Quaternion rot,Vector3 pos)
        {
            ShootClientClientRpc(NetworkObject.OwnerClientId, hitPoint, rot,pos);
        }

        [ClientRpc]
        void ShootClientClientRpc(ulong id, Vector3 hitPoint,Quaternion rot,Vector3 pos)
        {
            if (id != NetworkObject.OwnerClientId || NetworkManager.Singleton.LocalClientId==id) return;
            //Debug.LogError(_context.CurrentWeapon);

            _context.CurrentWeapon.FireBullet(hitPoint,rot,pos);
        }



        private void LateUpdate()
        {
            if (isDataSet && !Context.health.isDead)
            {
                _ikHandler.Schedule();
            }
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

        bool weaponSet = false;

        internal void SetWeaponData(int weaponindex,int layer)
        {
            GameObject Weapon = Instantiate(ItemReference.Instance.weaponsData.Weapons[weaponindex]);
            if (IsOwner) Weapon.GetComponent<WBWeapon>().SetScopeCamera(_context.PlayerScopeCamera.transform);
            _context.ScopeOnRatio = Weapon.GetComponent<WBWeapon>().Data.TouchRatioOnScope;
            Context.Inventory.AddItem(new WBItem
            {
                ItemName = Weapon.GetComponent<WBWeapon>().Data.AmmoType,
                ItemType = WBItemType.Bullet,
                ItemAmount = Weapon.GetComponent<WBWeapon>().Data.MagSize
            });
            Weapon.GetComponent<WBWeapon>().Setpool(layer, NetworkObject.OwnerClientId);
            SetWeapon(Weapon);
            if (IsOwner)
            {
                Weapon.GetComponent<WBWeapon>().SetFieldView();
                Invoke(nameof(SetLookLookRotationOnWeapon), 0.5f);
                _context.UpdateAmmo(_context.CurrentWeapon);
            }
        }

        void SetLookLookRotationOnWeapon()
        {
            if (Physics.Raycast(_context.PlayerCamera.transform.position,
                           _context.PlayerCamera.transform.forward,
                           out RaycastHit _hit,
                            Mathf.Infinity,
                            _context.Data.DamageLayer))
            {
                //Debug.LogError(_context.CurrentWeapon);
                _context.CurrentWeapon.transform.LookAt(_hit.point);
            }
        }

        

        internal void SetWeapon(GameObject weapon)
        {
            if(!isDataSet) setContext();
            _context.CurrentPickUpItem = weapon.transform;
            //Debug.LogError(_context.CurrentPickUpItem);
           
           
            OnItemEquip();
            //Debug.LogError(gameObject.name);
            //Debug.Log(_context);
            //Debug.Log(_context.CurrentWeapon);
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
                foreach (Transform item in Context.WeaponSlots.RightHandReference)
                {
                    Destroy(item.gameObject);
                    
                }
                _context.CurrentWeapon = null;
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

        internal void AddDamage(int damage,ulong clientId)
        {
            AddDamageServerRpc(clientId, damage);
        }

        [ServerRpc (RequireOwnership =false)]
        void AddDamageServerRpc(ulong id,int damage)
        {
            //Debug.LogError("Damaging");
            AddDamageClientRpc(id, damage);
        }


        [ClientRpc]
        void AddDamageClientRpc(ulong id,int damage)
        {
            if (NetworkObject.OwnerClientId == id)
            {
                GetComponent<HealthManager>().AddDamage(damage); 
            }
        }

        internal void SetKill()
        {
            mykills++;
            WBUIActions.UpdatelocalScore?.Invoke(mykills);
            FindObjectOfType<ScoreManager>().SetTeamScoreScoreServerRpc(1, CustomProperties.Instance.isRed);
        }

        public void SetScope()
        {
            Context.isScopeOn = !Context.isScopeOn;
            SetLookLookRotationOnWeapon();
            PlayerSetManager.instance.ChangeView(Context.isScopeOn);
        }

        private void OnDestroy()
        {
        }

    }
}