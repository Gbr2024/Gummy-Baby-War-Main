using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using WeirdBrothers.ThirdPersonController;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] EnemyAi controller; 
    [SerializeField] float WeaponIndex;
    [SerializeField] Transform Front;
    private float AIFOV = 30;
    public WBWeapon weapon;
    public string AIname;

    public NetworkVariable<bool> isRed = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isaiming = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // Start is called before the first frame update
    void Start()
    {
        controller.players = FindTarget();
        weapon.AIname = AIname;
        weapon.isAI = true;
        weapon.Setpool(12, NetworkObject.OwnerClientId);
        animator.SetFloat("WeaponIndex", WeaponIndex);


    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        isaiming.OnValueChanged += (previous, current) => SetisAim(isaiming.Value);
        if (isRed.Value)
        {
            gameObject.layer = 10;
            controller.whatIsPlayer = 13;
        }
        else
        { 
            gameObject.layer = 13;
            controller.whatIsPlayer = 10;
        }
        isRed.OnValueChanged += (previous, current) => SetisRed(isRed.Value);
        SetSkin(LobbyManager.Instance.getSkinColor(isRed.Value));
    }

    private void SetisRed(bool value)
    {
        SetSkin(LobbyManager.Instance.getSkinColor(value));
    }

    private void SetisAim(bool value)
    {
       if(value)
        {
            animator.SetFloat("Aim", 1f);
        }
       else
        {
            animator.SetFloat("Aim", 0);
        }
    }

    internal void AddDamage(float damage, ulong ownerClientId, ulong playerID)
    {
        GetComponent<AIHealth>().AddDamage(damage, playerID);
    }

    void CalculateFOV()
    {
        Gizmos.color = Color.red;
        float totalFOV = AIFOV * 2;
        float rayRange = 10.0f;
        float halfFOV = totalFOV / 2.0f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.forward);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.forward);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        Gizmos.DrawRay(Front.position, leftRayDirection * rayRange);
        Gizmos.DrawRay(Front.position, rightRayDirection * rayRange);
    }

    private void OnDrawGizmos()
    {
        //CalculateFOV();
    }

    bool isAiming = false;
    public NetworkVariable<int> bulletlayer = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    public Vector3 velocity { get; private set; }

    private void Update()
    {
        velocity = transform.InverseTransformDirection(agent.velocity);
        animator.SetFloat("Hor", velocity.x);
        animator.SetFloat("Ver", velocity.z);

        if(controller.players.Count==0 && !ScoreManager.Instance.GameHasFinished)
            controller.players = FindTarget();

        if (controller.nearestPlayer!=null)
        {
            Debug.DrawLine(Front.position, Front.position+(Front.forward*15f), Color.red);
            if (Physics.Raycast(Front.position, Front.forward, out RaycastHit hit, 35f))
            {
               
                if ((hit.transform == controller.nearestPlayer || hit.transform.root == controller.nearestPlayer) && !isAiming)
                {
                    if (controller.nearestPlayer.TryGetComponent(out HealthManager H)) if (H.isDead) return;
                    if (controller.nearestPlayer.TryGetComponent(out AIHealth A)) if (A.isDead) return;
                    animator.SetFloat("Aim", 1f);
                    isAiming = true;
                }
                if(isAiming)
                {
                    if (controller.nearestPlayer.TryGetComponent(out HealthManager H)) if (!H.isDead) return;
                    if (controller.nearestPlayer.TryGetComponent(out AIHealth A)) if (!A.isDead) return;
                    animator.SetFloat("Aim", 0);
                    isAiming = false;
                }
                if(isAiming && Time.time>weapon._nextFire)
                {
                    shootClientRpc(hit.point,AIname);
                }
                if(weapon.CurrentAmmo==0)
                {
                    weapon.AddAmmo(weapon.Data.MagSize);
                }
            }

        }

    }

    [ClientRpc]
    void shootClientRpc(Vector3 point,string name)
    {
        foreach (var item in FindObjectsOfType<PlayerController>())
        {
            if(item.AIname==name)
                weapon.FireBullet(point, Quaternion.identity, weapon.GetMuzzleFlah.position, isRed.Value, true);
        }
        
    }

    private List<Transform> FindTarget()
    {
        List<Transform> ts = new();
        foreach (var item in FindObjectsOfType<WBThirdPersonController>())
        {
            if (item.isRed != isRed.Value)
                ts.Add(item.transform);
        }
        foreach (var item in FindObjectsOfType<PlayerController>())
        {
            if (item.isRed != isRed)
                ts.Add(item.transform);
        }
        return ts;
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
}
