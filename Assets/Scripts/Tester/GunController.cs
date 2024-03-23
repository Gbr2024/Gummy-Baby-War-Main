using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using WeirdBrothers.ThirdPersonController;

public class GunController : MonoBehaviour
{
    [SerializeField] Transform Muzzle;
    [SerializeField] int Magsize = 50, Damage = 30;
    [SerializeField] float FireRate = .5f;
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] ParticleSystem _muzzelFlash;
    // Start is called before the first frame update
    [SerializeField] private Bullet[] pool;
    [SerializeField] Transform lookTarget;
    private float _nextFire;
    internal int _currentAmmo;
    private int poolIndex;

    private void Start()
    {
        Setpool();
        _currentAmmo = Magsize;
    }

    internal void Setpool()
    {
        pool = new Bullet[Magsize + (int)Magsize / 2];
        for (int i = 0; i < Magsize + ((int)Magsize / 2); i++)
        {
            pool[i] = Instantiate(bulletPrefab);
            pool[i].gameObject.SetActive(false);
            pool[i].damage = Damage;
        }
    }

    internal void setLookTarget(Transform transform)
    {
        lookTarget = transform;
    }

    public void FireBullet( bool isRed,ulong playerID)
    {
        if (_currentAmmo <= 0 || lookTarget==null)
        { 
            return; 
        }
        if (Time.time > _nextFire)
        {
            _currentAmmo--;
            _nextFire = Time.time + FireRate;
            pool[poolIndex].gameObject.SetActive(false);
            pool[poolIndex].resetBullet();
            pool[poolIndex].transform.position = Muzzle.position;
            pool[poolIndex].transform.LookAt(lookTarget);
            pool[poolIndex].gameObject.SetActive(true);
            pool[poolIndex].isRed = isRed;
            pool[poolIndex].PlayerID = playerID;
            pool[poolIndex].moveBullet();
            poolIndex++;
            if (poolIndex >= pool.Length) poolIndex = 0;
        }
    }

    private void Update()
    {
        if (lookTarget != null)
            transform.LookAt(lookTarget);
    }

    private void OnDestroy()
    {
        foreach (var item in pool)
        {
            Destroy(item.gameObject);
        }
    }

}
