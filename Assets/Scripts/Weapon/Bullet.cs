using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    internal int damage;
    internal ulong PlayerID;
    internal string AIname;
    [SerializeField] private float range;
    [SerializeField] private Rigidbody rb;
    [SerializeField] TrailRenderer trail;
    [SerializeField] GameObject HitEffect;
    [SerializeField] GameObject Body;
    [SerializeField] bool DisableBody;

    [SerializeField] private float customGravity;
    internal bool isRed;
    internal bool isAI;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        
    }
    public void SetupBullet(float damage, float range)
    {
        this.damage =(int)damage;
        this.range = range;
    }



    internal void moveBullet(Vector3 point,Quaternion RottoUSe)
    {
       
        if (point != Vector3.zero)
        {
            float distanceToHit = range;
            // Adjust the force based on the distance and slowing distance
            float adjustedForce = Mathf.Clamp(distanceToHit, 0f, 1f) * range;
            transform.LookAt(point);
            // Apply the force to the rigidbody
            rb.AddForce(transform.forward * adjustedForce, ForceMode.Impulse);
        }
        else
        {
            transform.rotation = RottoUSe;
            rb.AddForce(transform.forward * range, ForceMode.Impulse);
        }
    }

    

    internal void moveBullet()
    {
        rb.AddForce(transform.forward * range, ForceMode.Impulse);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(trail.enabled)
        {
            if(Body!=null)Body.SetActive(!DisableBody);
            trail.enabled = false;
            Invoke(nameof(Sleep), 2f);
            if(TryGetComponent(out AudioSource ad))
                ad.Play();
            Instantiate(HitEffect, transform.position, Quaternion.identity);

        }
        
    }

    private void Sleep()
    {
        gameObject.SetActive(false);
    }

    internal void resetBullet()
    {
        trail.Clear();
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.angularDrag = 0;
        rb.drag = 0;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        if (DisableBody) Body.SetActive(true);
        trail.enabled = true;
    }

    //private void FixedUpdate()
    //{
    //    if (trail.enabled) rb.AddForce(Vector3.up * customGravity, ForceMode.Acceleration);
    //}
}

