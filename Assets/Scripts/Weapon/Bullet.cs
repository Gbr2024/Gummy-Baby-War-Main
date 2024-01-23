using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private Rigidbody rb;
    [SerializeField] TrailRenderer trail;
    [SerializeField] GameObject HitEffect;
    [SerializeField] GameObject Body;
    [SerializeField] bool DisableBody;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        
    }
    public void SetupBullet(float damage, float range)
    {
        this.damage = damage;
        this.range = range;
    }

    private void OnEnable()
    {
        trail.Clear();
        rb = GetComponent<Rigidbody>();
        rb.angularDrag = 0;
        rb.drag = 0;
        rb.velocity = Vector3.zero;
        if(DisableBody)Body.SetActive(true);
        rb.AddForce(transform.forward * range, ForceMode.Impulse);
        trail.enabled = true;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(trail.enabled)
        {
            if(Body!=null)Body.SetActive(!DisableBody);
            trail.enabled = false;
            Invoke(nameof(Sleep), 4f);
            Instantiate(HitEffect, transform.position, Quaternion.identity);
        }
        
    }

    private void Sleep()
    {
        gameObject.SetActive(false);
    }
}

