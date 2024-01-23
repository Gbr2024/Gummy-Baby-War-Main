using UnityEngine;

namespace WeirdBrothers.ThirdPersonController
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "WeirdBrothers/WeaponData")]
    public class WBWeaponData : ScriptableObject
    {
        [Space]
        [Header("Weapon UI")]
        public Sprite WeaponImage;
        public string WeaponName;

        [Space]
        [Header("Weapon Equip")]
        public WBWeaponPositionData WeaponHandPosition;
        public WBWeaponPositionData WeaponSlotPosition;
        public WBWeaponPositionData WeaponSlot2Position;

        [Space]
        [Header("Weapon Data")]
        public WBWeaponType WeaponType;
        public int WeaponIndex;
        public string AmmoType;
        public FireType FireType;
        public Bullet bullet;
        public GameObject BulletShell;
        public float BulletEjectingSpeed;
        public GameObject BulletHole;

        [Space]
        [Header("Weapon Recoil Data")]
        public float CrossHairSpread;
        public float RecoilDuration;
        public float VerticalRecoil;
        public float HorizontalRecoil;

        [Space]
        public float WeaponSpread;


        [Space]
        [Header("Weapon Fire Data")]
        public float Damage;
        public float FireRate;
        public int MagSize;
        public float Range;

        [Space]
        [Header("Weapon Audio")]
        public AudioClip FireSound;
        public AudioClip MagOutSound;
        public AudioClip MagInSound;
        public AudioClip BoltSound;
    }
}
