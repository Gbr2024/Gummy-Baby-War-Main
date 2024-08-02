using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using WeirdBrothers.ThirdPersonController;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] EnemyAi controller;
    [SerializeField] float WeaponIndex;
    [SerializeField] Transform Front;
    [SerializeField] bool Debugmessage;
    [SerializeField] marker Marker;
    [SerializeField] AIHealth aIHealth;

    private float AIFOV = 30;
    public WBWeapon weapon;
    public string AIname;

    public EnemyAi GetEnemyAi { get { return controller; } }

    public NetworkVariable<bool> isRed = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isaiming = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // Start is called before the first frame update
    void Start()
    {
        controller.players = FindTarget();
        weapon.AIname = AIname;
        weapon.isAI = true;

        animator.SetFloat("WeaponIndex", WeaponIndex);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        isaiming.OnValueChanged += (previous, current) => SetisAim(isaiming.Value);
        isRed.OnValueChanged += (previous, current) => SetisRed(isRed.Value);
        int n = 9;
        if (isRed.Value == true)
        {
            gameObject.layer = 10;
        }
        else
        {
            gameObject.layer = 13;
            n = 12;
        }
      
        SetSkin(LobbyManager.Instance.getSkinColor(isRed.Value));
        controller.players = FindTarget();
        weapon.Setpool(n, NetworkObject.OwnerClientId);
    }

    private void SetisRed(bool value)
    {
        SetSkin(LobbyManager.Instance.getSkinColor(value));
        if (value == true)
        {
            gameObject.layer = 10;
            weapon.Setpool(9, NetworkObject.OwnerClientId);
        }
        else
        {
            gameObject.layer = 13;
            weapon.Setpool(12, NetworkObject.OwnerClientId);
        }

        if (CustomProperties.Instance.isRed == value)
            Marker.EnableBody(true, false);
    }

    private void SetisAim(bool value)
    {
        if (value)
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

    public Vector3 localVelocity { get; private set; }
    Vector3 normalizedVelocity;
    float smoothHor, smoothVer;
    float smoothingSpeed = 1f;

    bool aim = false;
    private void Update()
    {
        if (weapon.CurrentAmmo == 0)
        {
            weapon.AddAmmo(weapon.Data.MagSize);
        }
        if (!IsServer) return;
        if (aIHealth.isDead) return;

        localVelocity = transform.InverseTransformDirection(agent.velocity);

        // Normalize velocity to prevent unnatural speed changes
        normalizedVelocity = localVelocity.normalized;

        // Smooth transitions for animation parameters
        smoothHor = Mathf.Lerp(animator.GetFloat("Hor"), normalizedVelocity.x, Time.deltaTime * smoothingSpeed);
        smoothVer = Mathf.Lerp(animator.GetFloat("Ver"), normalizedVelocity.z, Time.deltaTime * smoothingSpeed);

        // Set animator parameters
        animator.SetFloat("Hor", smoothHor);
        animator.SetFloat("Ver", smoothVer);

        if (controller.players.Count == 0 && !ScoreManager.Instance.GameHasFinished)
            controller.players = FindTarget();

        if (controller.nearestPlayer != null)
        {
            if (controller.playerInAttackRange && controller.CanSeePlayer)
            {
                if (controller.nearestPlayer.TryGetComponent(out HealthManager H))
                {
                    if (!H.isDead)
                        aim = true;
                    else
                    {
                        aim = false;
                        controller.nearestPlayer = null;
                    }
                }
                else if (controller.nearestPlayer.TryGetComponent(out AIHealth A))
                {
                    if (!A.isDead)
                        aim = true;
                    else
                    {
                        aim = false;
                        controller.nearestPlayer = null;
                    }
                }
                if (aim)
                {
                    animator.SetFloat("Aim", 1f);
                    isAiming = true;
                    isaiming.Value = true;
                }
                else
                {
                    animator.SetFloat("Aim", 0);
                    isAiming = false;
                    isaiming.Value = false;
                }
                if (isAiming && Time.time > weapon._nextFire)
                {
                    weapon._nextFire = Time.time + weapon.Data.FireRate;
                    shootServerRpc(new Vector3(controller.nearestPlayer.position.x, controller.nearestPlayer.position.y + 1.25f, controller.nearestPlayer.position.z), AIname);
                }

            }
        }

    }

    internal void DisableAgent()
    {
        agent.enabled = false;
    }

    [ServerRpc]
    void shootServerRpc(Vector3 point, string name)
    {
        foreach (var item in FindObjectsOfType<PlayerController>())
        {
            if (item.AIname == name)
                try
                {
                    weapon.FireBullet(point, Quaternion.identity, weapon.GetMuzzleFlah.position, isRed.Value, true);
                }
                catch
                {
                    //nothing
                }
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
            if (item.isRed.Value != isRed.Value)
                ts.Add(item.transform);
        }
        return ts;
    }

    internal void SetSkin(int color)
    {
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
        if (CustomProperties.Instance.isRed == isRed.Value)
            Marker.SetColor(Color.blue);
        else
            Marker.SetColor(Color.red);
    }
}
