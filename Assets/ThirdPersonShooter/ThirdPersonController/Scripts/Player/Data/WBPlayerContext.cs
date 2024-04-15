using System;
using UnityEngine;
using Cinemachine;
using WeirdBrothers.IKHepler;
using WeirdBrothers.CharacterController;

namespace WeirdBrothers.ThirdPersonController
{
    [Serializable]
    public class WBPlayerContext
    {
        //private properties
        [SerializeField] private WBPlayerData _data;
        [SerializeField] private TrajectoryController trajectoryController;

        public Transform GrenadeHandPos;
        public int GrenadeCount = 1;
        public WBPlayerData Data => _data;
        public TrajectoryController trajectory => trajectoryController;

        [SerializeField] private LookAtIK _weaponIK;
        public LookAtIK WeaponIK => _weaponIK;

        [Space]
        [SerializeField] private CrossHairSettings _crossHairSetting;

        public bool GrenadeSet { get; internal set; }

        public CrossHairSettings CrossHair => _crossHairSetting;

        [Space]
        [SerializeField] private CinemachineVirtualCamera _camera;

        private WBThirdPersonAnimator _animator;
        public WBThirdPersonAnimator Animator => _animator;

        private WBCharacterController _controller;
        public WBCharacterController Controller => _controller;

        private WBThirdPersonController _ShooterController;
        public WBThirdPersonController ShooterController => _ShooterController;

        private WBInputHandler _input;
        public WBInputHandler Input => _input;

        private WBItemPickUpManager _pickUpManager;
        public WBItemPickUpManager PickUpManager => _pickUpManager;

        private Camera _playerCamera;
        public Camera PlayerCamera => _playerCamera;

        private CinemachinePOV _pov;
        public CinemachinePOV Pov => _pov;

        private WBPlayerInventory _inventory;
        public WBPlayerInventory Inventory => _inventory;

        private Transform _transform;
        public Transform Transform => _transform;

        private WBWeaponHandler _weaponHandler;
        public WBWeaponHandler WeaponHandler => _weaponHandler;

        private WBWeaponSlots _weaponSlots;
        public WBWeaponSlots WeaponSlots => _weaponSlots;
        public HealthManager health;
        public float ScopeOnRatio;
        internal float WaitTime = 0;

        //Netcode Code 
        public Vector3 RpcLookPos;
        public Vector3 RpcSpineRotation;
        public bool isScopeOn = false;

        //public properties        
        [HideInInspector] public Transform CurrentPickUpItem;
        [HideInInspector] public WBWeapon CurrentWeapon;
        [HideInInspector] public float RecoilTime;
        [HideInInspector] public float Speed;
        internal bool isAiming = false;

        public void SetData(Transform transform)
        {
            _animator = new WBThirdPersonAnimator(transform);
            _controller = transform.GetComponent<WBCharacterController>();
            _input = transform.GetComponent<WBInputHandler>();
            _pov = _camera.GetCinemachineComponent<CinemachinePOV>();
            _pickUpManager = new WBItemPickUpManager();
            _playerCamera = Camera.main;

            _inventory = new WBPlayerInventory();
            health = transform.GetComponent<HealthManager>();
            _transform = transform;
            _weaponHandler = new WBWeaponHandler();
            _weaponSlots = GetWeaponSlots(transform);
            if(transform.TryGetComponent<WBThirdPersonController>(out WBThirdPersonController Controller))
            {
                _ShooterController = Controller;
            }
        }

        private WBWeaponSlots GetWeaponSlots(Transform transform)
        {
            Animator animator = transform.GetComponent<Animator>();
            var rightHandRef = animator.GetBoneTransform(HumanBodyBones.RightHand).Find("RightHandRef");
            var primarySlot1 = animator.GetBoneTransform(HumanBodyBones.Spine).Find("PrimarySlot1");
            var primarySlot2 = animator.GetBoneTransform(HumanBodyBones.Spine).Find("PrimarySlot2");
            var secondarySlot = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).Find("SecondarySlot");
            var meleeSlot = animator.GetBoneTransform(HumanBodyBones.Spine).Find("MeleeSlot");

            WBWeaponSlots weaponSlots = new WBWeaponSlots
            {
                RightHandReference = rightHandRef,
                PrimarySlot1 = primarySlot1,
                PrimarySlot2 = primarySlot2,
                SecondarySlot = secondarySlot,
                MeleeSlot = meleeSlot
            };
            return weaponSlots;
        }

        public void SetAnimator()
        {
            if (CurrentWeapon == null)
            {
                Animator.SetWeaponIdex(0);
            }
            else
            {
                int index = CurrentWeapon.Data.WeaponIndex;
                Animator.SetWeaponIdex(index);
            }
        }

        public void SetAnimator(int index)
        {
            Animator.SetWeaponIdex(index);
        }

        public void UpdateAmmo(WBWeapon weapon)
        {
            var index = 0;
            if (weapon.Data.WeaponType == WBWeaponType.Primary)
            {
                if (weapon.WeaponSlot == WeaponSlot.First)
                {
                    index = 1;
                }
                else if (weapon.WeaponSlot == WeaponSlot.Second)
                {
                    index = 2;
                }
            }
            else if (weapon.Data.WeaponType == WBWeaponType.Secondary)
            {
                index = 3;
            }
            else if (weapon.Data.WeaponType == WBWeaponType.Melee)
            {
                index = 4;
            }

            var weaponImage = weapon.gameObject.GetItemImage();
            var currentAmmo = weapon.CurrentAmmo;
            var totalAmmo = Inventory.GetAmmo(weapon.Data.AmmoType);
            if (ShooterController.IsOwner)
                WBUIActions.SetPrimaryWeaponUI?.Invoke(index, weaponImage, currentAmmo, totalAmmo);
        }

        public void UpdateAmmo()
        {
            WBWeapon[] weapons = Transform.GetComponentsInChildren<WBWeapon>();
            Array.ForEach(weapons, weapon =>
            {
                var index = 0;
                if (weapon.Data.WeaponType == WBWeaponType.Primary)
                {
                    if (weapon.WeaponSlot == WeaponSlot.First)
                    {
                        index = 1;
                    }
                    else if (weapon.WeaponSlot == WeaponSlot.Second)
                    {
                        index = 2;
                    }
                }
                else if (weapon.Data.WeaponType == WBWeaponType.Secondary)
                {
                    index = 3;
                }
                else if (weapon.Data.WeaponType == WBWeaponType.Melee)
                {
                    index = 4;
                }

                var weaponImage = weapon.gameObject.GetItemImage();
                var currentAmmo = weapon.CurrentAmmo;
                var totalAmmo = Inventory.GetAmmo(weapon.Data.AmmoType);
                if(ShooterController.IsOwner)
                    WBUIActions.SetPrimaryWeaponUI?.Invoke(index, weaponImage, currentAmmo, totalAmmo);
            });
        }

        public void GenerateRecoil(float time)
        {
            RecoilTime = time;
        }

        internal void setcamera(CinemachineVirtualCamera cam)
        {
            _camera = cam;
            //Debug.LogError("camera has set");
        }

        
    }
}