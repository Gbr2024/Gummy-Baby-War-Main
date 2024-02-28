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

        public WeaponSlot WeaponSlot;
        internal ulong id;

        [Space]
        public Transform LeftHandRef;

        [SerializeField]
        private Transform _shellSpawnPoint;
        
        [SerializeField]
        private Transform ScopeView;

        [SerializeField]
        private ParticleSystem _muzzelFlash;

        private int _currentAmmo;
        public int CurrentAmmo => _currentAmmo;

        public SmoothFollow CamController;
        private AudioSource _audioSource;
        private float _nextFire;
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
            _audioSource = GetComponent<AudioSource>();
            _currentAmmo = Data.MagSize;
            _directionToTarget = Vector3.zero;
            _hit = new RaycastHit();
            //Setpool();
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


        public void FireBullet(Vector3 hitPoint,Quaternion RotationToUse,Vector3 pos)
        {
            
            if (Time.time > _nextFire)
            {
                _currentAmmo--;
                pool[poolIndex].gameObject.SetActive(false);
                pool[poolIndex].resetBullet();
                _nextFire = Time.time + Data.FireRate;
                _audioSource.PlayOneShotAudioClip(Data.FireSound);
                pool[poolIndex].transform.position =pos;
                pool[poolIndex].transform.forward = transform.forward;
                pool[poolIndex].gameObject.SetActive(true);
                pool[poolIndex].moveBullet(hitPoint, RotationToUse);
                poolIndex++;
                
                if (poolIndex >= pool.Length) poolIndex = 0;

                if (Cylinder != null)
                {
                    WBUIActions.SetPrimaryWeaponUI?.Invoke(_index,Data.WeaponImage,
                    CurrentAmmo,
                    30);
                    //Debug.LogError(Cylinder.localEulerAngles.z);
                    Cylinder.DOLocalRotate(new Vector3(Cylinder.localEulerAngles.x, Cylinder.localEulerAngles.y, Cylinder.localEulerAngles.z - CylinderRotation), Data.FireRate*.8f); 
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
            PlayerSetManager.instance.ScopeCinemachine.Follow = CamController.transform;
        }

        internal void SetFieldView()
        {
            //PlayerSetManager.instance.GetScopeView.forward = ScopeView.forward;
            //PlayerSetManager.instance.GetScopeView.position = ScopeView.position;
            //PlayerSetManager.instance.GetScopeView.forward = ScopeView.forward;
            //PlayerSetManager.instance.SetScopeCamFeildView(Data.FeildView);
            //PlayerSetManager.instance.GetScopeView.GetComponent<ScopeCamMovement>().SetTarget(ScopeView);//
            //PlayerSetManager.instance.GetScopeView.SetParent(ScopeView);
            PlayerSetManager.instance.GetScopeView.forward = ScopeView.forward;
            //PlayerSetManager.instance.GetScopeView.localPosition = Vector3.zero;
            //PlayerSetManager.instance.GetScopeView.localRotation = Quaternion.identity;
            PlayerSetManager.instance.SetScopeCamFeildView(Data.FeildView);
        }

       

       
    }
}