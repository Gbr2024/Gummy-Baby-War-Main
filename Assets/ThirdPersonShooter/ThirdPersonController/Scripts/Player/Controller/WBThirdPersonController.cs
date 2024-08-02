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
        [SerializeField] bool test = false;
        [SerializeField] marker Marker;
        [SerializeField] private WBPlayerContext _context;
        public WBPlayerContext Context => _context;


        [SerializeField] private PlayerState[] _states;
        internal Syncer syncer;

        private List<object> _playerStates;
        private WBPlayerMovement _movement;
        private WBPlayerIKHandle _ikHandler;

       

        internal bool isRed = false;
        internal int bulletlayer;
        bool isDataSet = false;
        

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
                PlayerSetManager.instance.setAimCam(this);
                syncer = GetComponent<Syncer>();
                isRed = CustomProperties.Instance.isRed;
                SetSkin(LobbyManager.Instance.getSkinColor(isRed));
                syncer.isRed.Value = isRed;
                if (CustomProperties.Instance.isRed)
                    gameObject.layer = 10;
                else
                    gameObject.layer = 13;
                bulletlayer = isRed ? 9 : 12;

                SetWeaponData(PlayerPrefs.GetInt("WeaponIndex"), bulletlayer);
                syncer.WeaponIndex.Value = PlayerPrefs.GetInt("WeaponIndex");
                Context.isScopeOn = false;
                WBUIActions.isPlayerActive = true;
                Context.GrenadeCount = 1;
                WBUIActions.EnableGrenadeButton?.Invoke(true);
                WBUIActions.EnableTouch?.Invoke(true);
                FindObjectOfType<WBTouchLook>().context = Context;
            }
            else
            {
                GetComponent<WBInputHandler>().enabled = false;
                isRed = syncer.isRed.Value;
            }
            if (!IsLocalPlayer && CustomProperties.Instance.isRed == isRed)
            {
                GetComponentInChildren<marker>().SetColor(Color.blue);
                GetComponentInChildren<marker>().EnableBody(true, false);
            }
            else
                GetComponentInChildren<marker>().SetColor(Color.red);

            transform.LookAt(ItemReference.Instance?.EmtptyTarget);
        }

        

        internal void resetAim()
        {
            CancelInvoke(nameof(ResetAim));
            Invoke(nameof(ResetAim), Context.CurrentWeapon.Data.RecockTime);
        }

        private void ResetAim()
        {
            SetScope(_context.isScopeOn);
        }

        internal void setContext()
        {
            PlayerSetManager.instance.SetCamera(this);
            _context.SetData(transform);
            _states = _states.Distinct().ToArray();
            _playerStates = new List<object>();
            _movement = new WBPlayerMovement(_context);
            _ikHandler = new WBPlayerIKHandle(_context);
            _context.GrenadeSet = false;

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

        //private void FixedUpdate()
        //{
        //    if (!IsOwner) return;
        //    if(Context.CurrentWeapon!=null && Context.PlayerScopeCamera!=null)
        //    {
        //        Context.PlayerScopeCamera.transform.position=Context.CurrentWeapon.ScopeView.position;
        //        Context.PlayerScopeCamera.transform.forward=Context.CurrentWeapon.ScopeView.forward;
        //    }
        //}

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
            if (IsOwner && transform.position.y < ItemReference.Instance?.hasgoneDownY)
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                
            
            if (IsOwner && isDataSet && !Context.health.isDead)
            {
                if(ScoreManager.Instance.GameHasFinished && Context.CurrentWeapon.ScopeView.gameObject.activeSelf)
                {
                    Context.CurrentWeapon.ScopeView.gameObject.SetActive(false);
                }
                if (test)
                {
                    test = false;
                    SetKillStreakClientRPC(Random.Range(2, 6), OwnerClientId);
                }
                _movement.Schedule();
                CustomProperties.Instance.LastPositon = transform.position;
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

        internal void DespawnGrenade()
        {
            PlayerCreator.Instance.DespawnGrenadeServerRpc(OwnerClientId);
        }

        internal void CreateGrenade()
        {
            //Debug.LogError("Being Called");
            PlayerCreator.Instance.CreateGrenadeServerRpc(OwnerClientId,isRed);
        }

        bool AimEnd = false;

        internal void FireInAll(Vector3 hitPoint, LayerMask damageLayer)
        {
            if (!AimEnd && !Context.isScopeOn) return;
            var camRot = Camera.main.transform.rotation;
           
            if(_context.CurrentWeapon.CurrentAmmo>0)
            {
                ShootServerRpc(NetworkObject.OwnerClientId, hitPoint, camRot,Context.CurrentWeapon.GetMuzzleFlah.position,isRed);
                _context.CurrentWeapon.FireBullet(hitPoint, camRot, Context.CurrentWeapon.GetMuzzleFlah.position,isRed,true);
            }
            if(_context.CurrentWeapon.Data.isSniper && Context.isScopeOn)
            {
                Context.Animator._animator.SetTrigger("DummyReload");
                Context.isRecoking = true;
                Context.CurrentWeapon.ScopeView.gameObject.SetActive(false);
                WBUIActions.EnableSecShoot?.Invoke(false);
                Invoke(nameof(ResetScope), 1.5f);
                Invoke(nameof(PlayRecock), .5f);
                
            }

        }

        public void ResetScope()
        {
            
            if (Context.CurrentWeapon.CurrentAmmo == 0)
               Context.isScopeOn = false;
            Context.isRecoking = false;
            SetScope(Context.isScopeOn);
            
        }
        public void PlayRecock()
        {
            Context.CurrentWeapon.MagOut();
        }


        

        internal void NewFire(Vector3 hitPoint, LayerMask damageLayer)
        {
            StartCoroutine(shootWait(hitPoint, damageLayer));
        }

        IEnumerator shootWait(Vector3 hitPoint, LayerMask damageLayer)
        {
            AimEnd = false;
            yield return new WaitForSeconds(0.1f);
            AimEnd = true;
            FireInAll(hitPoint, damageLayer);

        }

        [ServerRpc (RequireOwnership =false)]
        void ShootServerRpc(ulong id, Vector3 hitPoint, Quaternion rot,Vector3 pos,bool isRed)
        {
            ShootClientClientRpc(NetworkObject.OwnerClientId, hitPoint, rot,pos,isRed);
        }

        

        [ClientRpc]
        void ShootClientClientRpc(ulong id, Vector3 hitPoint,Quaternion rot,Vector3 pos,bool isRed)
        {
            if (id != NetworkObject.OwnerClientId || NetworkManager.Singleton.LocalClientId==id) return;
            //Debug.LogError(_context.CurrentWeapon);

            _context.CurrentWeapon.FireBullet(hitPoint,rot,pos,isRed);
        }





        private void LateUpdate()
        {
            if (isDataSet && !Context.health.isDead)
            {
                _ikHandler.Schedule();
            }
            if(_context.CurrentWeapon!=null)
            {
                if (_context.CurrentWeapon.transform.localPosition != _context.CurrentWeapon.Data.WeaponHandPosition.Position) _context.CurrentWeapon.transform.localPosition = _context.CurrentWeapon.Data.WeaponHandPosition.Position;
                if (_context.CurrentWeapon.transform.localRotation != Quaternion.Euler(_context.CurrentWeapon.Data.WeaponHandPosition.Rotation)) _context.CurrentWeapon.transform.localRotation = Quaternion.Euler(_context.CurrentWeapon.Data.WeaponHandPosition.Rotation);
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
            _context.ScopeOnRatio = Weapon.GetComponent<WBWeapon>().Data.TouchRatioOnScope;
            Context.Inventory.AddItem(new WBItem
            {
                ItemName = Weapon.GetComponent<WBWeapon>().Data.AmmoType,
                ItemType = WBItemType.Bullet,
                ItemAmount = Weapon.GetComponent<WBWeapon>().Data.MagSize
            });
            Weapon.GetComponent<WBWeapon>().Setpool(layer, NetworkObject.OwnerClientId);
            SetWeapon(Weapon);
            Weapon.GetComponent<WBWeapon>().setStartData();
            if (IsOwner)
            {
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

            if (_context.CurrentWeapon != null && !_context.GrenadeSet && _context.CurrentWeapon.Body.activeSelf && Context.isAiming)
            {
                var handRef = _context.CurrentWeapon.LeftHandRef;
                if (handRef == null) return;

                _context.Animator.setLeftHand(handRef.position, handRef.rotation);
                return;
            }
        }

        internal void AddDamage(float damage,ulong clientId, ulong playerID)
        {
            AddDamageServerRpc(clientId, damage,playerID);
        }
        internal void AddDamage(float damage,ulong clientId, string AIname, bool isAI=false)
        {
            AddDamageServerRpc(clientId, damage, AIname,isAI);
        }

        [ServerRpc (RequireOwnership =false)]
        void AddDamageServerRpc(ulong id,float damage, ulong playerID)
        {
            //Debug.LogError("Damaging");
            AddDamageClientRpc(id, damage,playerID);
        }
        
        [ServerRpc (RequireOwnership =false)]
        void AddDamageServerRpc(ulong id,float damage,string AIname, bool playerID)
        {
            //Debug.LogError("Damaging");
            AddDamageClientRpc(id, damage,0, AIname,playerID);
        }


        [ClientRpc]
        void AddDamageClientRpc(ulong id,float damage, ulong playerID,string name,bool isAI=false)
        {
            if (NetworkObject.OwnerClientId == id)
            {
                GetComponent<HealthManager>().AddDamage(damage,playerID, name, isAI); 
            }
        }
        
        [ClientRpc]
        void AddDamageClientRpc(ulong id,float damage, ulong playerID,bool isAI=false)
        {
            if (NetworkObject.OwnerClientId == id)
            {
                GetComponent<HealthManager>().AddDamage(damage,playerID,"",isAI); 
            }
        }

        

        public void SetScope(bool b)
        {
            if (PlayerSetManager.instance.scopemoving) return;
            Context.isScopeOn = b;
            Context.isAiming = Context.isScopeOn;
            Context.Animator.SetAim(Context.isScopeOn);
            SetLookLookRotationOnWeapon();
            Debug.LogError(Context.isScopeOn);
            WBUIActions.EnableSecShoot?.Invoke(Context.isScopeOn);
            if (Context.CurrentWeapon.Data.isSniper)
                Context.CurrentWeapon.ScopeView.gameObject.SetActive(Context.isScopeOn);
            else
                PlayerSetManager.instance.ChangeView(Context.isScopeOn ? Context.CurrentWeapon.Data.FeildView : 40f);
        }

        internal void SetSkin(int color)
        {
            //Debug.LogError("On Value Invoked +" + gameObject.name);
            List<Material> mats = new List<Material>();
            foreach (var item in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                foreach (var item2 in item.materials)
                {
                    mats.Add(item2);
                }
            }
            foreach (var item in mats)
            {
                item.SetColor("_BaseColor", ItemReference.Instance.colorReference.CharacterColors[color].color);
            }
        }

        internal void ActivateBodyOnWait()
        {
            Invoke(nameof(ActivateBody), .5f);
        }
        void ActivateBody()
        {
            if (_context.CurrentWeapon == null) return;
            _context.CurrentWeapon.Body.SetActive(true);
            _context.ShooterController.syncer.isWeaponActivated.Value = true;
        }

        [ClientRpc]
        internal void SetKillStreakClientRPC(int KillStreak,ulong clientID)
        {
            if( IsOwner && OwnerClientId==clientID)
            {
                PlayerCreator.Instance.killstreak = KillStreak;
                if(PlayerCreator.Instance.killstreak >= 2)
                {
                    WBUIActions.EnableKillstreakButton?.Invoke(true);
                    WBUIActions.ChangeKillstreak?.Invoke(PlayerCreator.Instance.killstreak.ToString());
                }
            }
        }
        
        [ClientRpc]
        internal void BreakCameraClientRPC(ulong clientID)
        {
            if( IsOwner && OwnerClientId==clientID)
            {
                WBUIActions.EnableBrokenScreen?.Invoke(true);
            }
        }



       

    }
}