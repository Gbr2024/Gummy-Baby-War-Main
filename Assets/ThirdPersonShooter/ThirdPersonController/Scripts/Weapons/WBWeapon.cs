using UnityEngine;
using DG.Tweening;

namespace WeirdBrothers.ThirdPersonController
{
    public enum WeaponSlot
    {
        None,
        First,
        Second
    }

    public class WBWeapon : MonoBehaviour, IItemImage, IItemName
    {
        [Header("Weawpon Data")]
        [Space]
        public WBWeaponData Data;
        public GameObject Body;
        public bool isAI = false;
        public string AIname;

        public WeaponSlot WeaponSlot;
        internal ulong id;

        [Space]
        public Transform LeftHandRef;

        [SerializeField]
        private Transform _shellSpawnPoint;
        
        public Transform ScopeView;

        [SerializeField]
        private ParticleSystem _muzzelFlash;

        private int _currentAmmo;
        public int CurrentAmmo => _currentAmmo;

        public SmoothFollow CamController;
        private AudioSource _audioSource;
        internal float _nextFire;
        private int _index;
        private Vector3 _directionToTarget;
        private RaycastHit _hit;

        internal Transform GetMuzzleFlah { get { return _muzzelFlash.transform;} }

        [SerializeField] private Bullet[] pool;
        private int poolIndex = 0;
        bool isfiring = false;

        [Header("Cylinder")]
        [Space]
        [SerializeField] Transform Cylinder;
        [SerializeField] float CylinderRotation;

       


        private void Start()
        {
            setStartData();
            //Setpool();
            _nextFire = Time.time + Data.FireRate;
        }
        
        internal void setStartData()
        {
            _audioSource = GetComponent<AudioSource>();
            _directionToTarget = Vector3.zero;
            _hit = new RaycastHit();
            _currentAmmo = Data.MagSize;
        }

        private void FixedUpdate()
        {
            //if(isfiring)
            //{
            //    FireBullet();
            //}

           
        }

        public void setfiring(bool b)
        {
            isfiring = b;
        }

        internal void Setpool(int layer,ulong ID)
        {
            if(pool.Length>0)
            {
                foreach (var item in pool)
                {
                    if(item!=null)
                    {
                        Destroy(item.gameObject);
                    }
                    
                }
            }
            
            id = ID;
            pool = new Bullet[Data.MagSize+ (int)Data.MagSize/2];
            for (int i = 0; i < Data.MagSize + ((int)Data.MagSize / 2); i++)
            {
                pool[i] = Instantiate(Data.bullet).GetComponent<Bullet>();
                pool[i].gameObject.layer = layer;
                pool[i].gameObject.SetActive(false);
                pool[i].PlayerID = id;
                pool[i].damage =(int) Data.Damage;
            }
        }

        public void RemoveRigidBody()
        {
            Destroy(GetComponent<Rigidbody>());
        }

        public Sprite GetItemImage()
        {
            return Data.WeaponImage;
        }

        public string GetItemName()
        {
            return Data.WeaponName;
        }

        public void Fire(Vector3 hitPoint, LayerMask DamageLayer)
        {
            if (Time.time > _nextFire)
            {
                if (_currentAmmo > 0)
                {
                    _nextFire = Time.time + Data.FireRate;
                    _currentAmmo--;
                    _audioSource.PlayOneShotAudioClip(Data.FireSound);
                    OnCaseOut();
                    _muzzelFlash.Play();

                    if (hitPoint != Vector3.zero)
                    {
                        _directionToTarget = hitPoint - _muzzelFlash.transform.position;
                        if (Physics.Raycast(_muzzelFlash.transform.position,
                            CalculatelateSpread(Data.WeaponSpread, _directionToTarget), out _hit,
                            Data.Range, DamageLayer))
                        {
                            OnTargetHit(_hit);
                        }
                    }
                }
            }
        }


        public void FireBullet(Vector3 hitPoint,Quaternion RotationToUse,Vector3 pos,bool isRed,bool isOwner=false)
        {
            if(!Data.IsShotGun)
            {
                if (Time.time > _nextFire)
                {
                    if (poolIndex >= pool.Length) poolIndex = 0;
                    _nextFire = Time.time + Data.FireRate;
                    _currentAmmo--;
                    pool[poolIndex].gameObject.SetActive(false);
                    pool[poolIndex].resetBullet();
                    if (_audioSource != null)
                    {
                        _audioSource.clip = Data.FireSound;
                    }
                    _audioSource.Play();
                    pool[poolIndex].transform.position = pos;
                    pool[poolIndex].transform.forward = transform.forward;
                    pool[poolIndex].gameObject.SetActive(true);
                    pool[poolIndex].moveBullet(hitPoint, RotationToUse);
                    pool[poolIndex].isRed = isRed;
                    if (isRed) pool[poolIndex].gameObject.layer = 9;
                    else pool[poolIndex].gameObject.layer = 12;
                    pool[poolIndex].isAI = isAI;
                    pool[poolIndex].AIname = AIname;
                    poolIndex++;
                    if (isOwner) CustomProperties.Instance.currentAmmo = _currentAmmo;
                    if (poolIndex >= pool.Length) poolIndex = 0;

                    if (Cylinder != null)
                    {
                        //Debug.LogError(Cylinder.localEulerAngles.z);
                        Cylinder.DOLocalRotate(new Vector3(Cylinder.localEulerAngles.x, Cylinder.localEulerAngles.y, Cylinder.localEulerAngles.z - CylinderRotation), Data.FireRate * .8f);
                    }
                }
            }
            else
            {
                _audioSource.Play();
                for (int i = 0; i < Data.ShotGunSlug; i++)
                {
                    // Randomize hit point slightly for spread effect
                    Vector3 randomOffset = new Vector3(
                        Random.Range(-Data.ShotGunSpread, Data.ShotGunSpread),
                        Random.Range(-Data.ShotGunSpread, Data.ShotGunSpread),
                        0
                    );
                    Vector3 adjustedHitPoint = hitPoint + randomOffset;

                    if (poolIndex >= pool.Length) poolIndex = 0;
                    _nextFire = Time.time + Data.FireRate; // You may want to adjust this for shotgun firing if needed
                    _currentAmmo--;

                    pool[poolIndex].gameObject.SetActive(false);
                    pool[poolIndex].resetBullet();
                    if (_audioSource != null)
                    {
                        _audioSource.clip = Data.FireSound;
                    }
                    
                    pool[poolIndex].transform.position = pos;
                    pool[poolIndex].transform.forward = transform.forward;
                    pool[poolIndex].gameObject.SetActive(true);
                    pool[poolIndex].moveBullet(adjustedHitPoint, RotationToUse);
                    pool[poolIndex].isRed = isRed;
                    pool[poolIndex].gameObject.layer = isRed ? 9 : 12;
                    pool[poolIndex].isAI = isAI;
                    pool[poolIndex].AIname = AIname;
                    poolIndex++;
                    if (isOwner) CustomProperties.Instance.currentAmmo = _currentAmmo;
                    if (poolIndex >= pool.Length) poolIndex = 0;

                    if (Cylinder != null)
                    {
                        Cylinder.DOLocalRotate(new Vector3(Cylinder.localEulerAngles.x, Cylinder.localEulerAngles.y, Cylinder.localEulerAngles.z - CylinderRotation), Data.FireRate * .8f);
                    }
                }
            }
        }
        public void FireBullets(Vector3 hitPoint, Quaternion RotationToUse, Vector3 pos, bool isRed, bool isOwner = false)
        {
            if (!Data.IsShotGun)
            {
                if (poolIndex >= pool.Length) poolIndex = 0;
                _nextFire = Time.time + Data.FireRate;
                _currentAmmo--;
                pool[poolIndex].gameObject.SetActive(false);
                pool[poolIndex].resetBullet();
                if (_audioSource != null)
                {
                    _audioSource.clip = Data.FireSound;
                }
                _audioSource.Play();
                pool[poolIndex].transform.position = pos;
                pool[poolIndex].transform.forward = transform.forward;
                pool[poolIndex].gameObject.SetActive(true);
                pool[poolIndex].moveBullet(hitPoint, RotationToUse);
                pool[poolIndex].isRed = isRed;
                if (isRed) pool[poolIndex].gameObject.layer = 9;
                else pool[poolIndex].gameObject.layer = 12;
                pool[poolIndex].isAI = isAI;
                pool[poolIndex].AIname = AIname;
                poolIndex++;
                if (isOwner) CustomProperties.Instance.currentAmmo = _currentAmmo;
                if (poolIndex >= pool.Length) poolIndex = 0;

                if (Cylinder != null)
                {
                    //Debug.LogError(Cylinder.localEulerAngles.z);
                    Cylinder.DOLocalRotate(new Vector3(Cylinder.localEulerAngles.x, Cylinder.localEulerAngles.y, Cylinder.localEulerAngles.z - CylinderRotation), Data.FireRate * .8f);
                }
            }
            else
            {
                _audioSource.Play();
                for (int i = 0; i < Data.ShotGunSlug; i++)
                {
                    // Randomize hit point slightly for spread effect
                    Vector3 randomOffset = new Vector3(
                        Random.Range(-Data.ShotGunSpread, Data.ShotGunSpread),
                        Random.Range(-Data.ShotGunSpread, Data.ShotGunSpread),
                        0
                    );
                    Vector3 adjustedHitPoint = hitPoint + randomOffset;

                    if (poolIndex >= pool.Length) poolIndex = 0;
                    _nextFire = Time.time + Data.FireRate; // You may want to adjust this for shotgun firing if needed
                    _currentAmmo--;

                    pool[poolIndex].gameObject.SetActive(false);
                    pool[poolIndex].resetBullet();
                    if (_audioSource != null)
                    {
                        _audioSource.clip = Data.FireSound;
                    }
                    pool[poolIndex].transform.position = pos;
                    pool[poolIndex].transform.forward = transform.forward;
                    pool[poolIndex].gameObject.SetActive(true);
                    pool[poolIndex].moveBullet(adjustedHitPoint, RotationToUse);
                    pool[poolIndex].isRed = isRed;
                    pool[poolIndex].gameObject.layer = isRed ? 9 : 12;
                    pool[poolIndex].isAI = isAI;
                    pool[poolIndex].AIname = AIname;
                    poolIndex++;
                    if (isOwner) CustomProperties.Instance.currentAmmo = _currentAmmo;
                    if (poolIndex >= pool.Length) poolIndex = 0;

                    if (Cylinder != null)
                    {
                        Cylinder.DOLocalRotate(new Vector3(Cylinder.localEulerAngles.x, Cylinder.localEulerAngles.y, Cylinder.localEulerAngles.z - CylinderRotation), Data.FireRate * .8f);
                    }
                }
            }
        }

        private void OnTargetHit(RaycastHit hit)
        {
            if (hit.transform.gameObject.GetComponent<IDamageable>() != null)
            {

            }
            else
            {
                GameObject bulletHole = Instantiate(Data.BulletHole, hit.point + (hit.normal * 0.01f), Quaternion.FromToRotation(-Vector3.forward, hit.normal));
                bulletHole.transform.SetParent(hit.transform);
                Destroy(bulletHole, 10);
            }
        }



        private Vector3 CalculatelateSpread(float inaccuracy, Vector3 direction)
        {
            if (inaccuracy == 0)
                return direction;
            direction.x += Random.Range(-inaccuracy, inaccuracy);
            direction.y += Random.Range(-inaccuracy, inaccuracy);
            direction.z += Random.Range(-inaccuracy, inaccuracy);
            return direction;
        }

        public void AddAmmo(int ammount)
        {
            _currentAmmo += ammount;
        }

        public void MagIn()
        {
            _audioSource.PlayOneShotAudioClip(Data.MagInSound);
        }

        public void MagOut()
        {
            _audioSource.PlayOneShotAudioClip(Data.MagOutSound);
        }

        public void Bolt()
        {
            _audioSource.PlayOneShotAudioClip(Data.BoltSound);
        }

        private void OnCaseOut()
        {
            GameObject ejectedCase = Instantiate(Data.BulletShell, _shellSpawnPoint.position, _shellSpawnPoint.rotation);
            Rigidbody caseRigidbody = ejectedCase.GetComponent<Rigidbody>();
            caseRigidbody.velocity = _shellSpawnPoint.TransformDirection(-Vector3.left * Data.BulletEjectingSpeed);
            caseRigidbody.AddTorque(Random.Range(-0.5f, 0.5f), Random.Range(0.2f, 0.3f), Random.Range(-0.5f, 0.5f));
            caseRigidbody.AddForce(0, Random.Range(2f, 4f), 0, ForceMode.Impulse);
            Destroy(ejectedCase, 5f);
        }

        internal void SetScopeCamera(Transform transform)
        {
            CamController.SetCam(transform);
            setStartData();
            //PlayerSetManager.instance.ScopeCinemachine.Follow = CamController.transform;
        }

       

        private void OnDestroy()
        {
            foreach (var item in pool)
            {
                if(item!=null)
                {
                    Destroy(item.gameObject);
                }
                
            }
        }


    }
}