using UnityEngine;

namespace WeirdBrothers.ThirdPersonController
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "WeirdBrothers/Player/Data")]
    public class WBPlayerData : ScriptableObject
    {
        [Space]
        [Header("Animation Smoothing")]
        [Range(0, 1f)]
        public float StartAnimTime = 0.3f;
        
        [Range(0, 1f)]
        public float AnimeSmoothTime = 0.3f;

        [Range(0, 1f)]
        public float StopAnimTime = 0.15f;

        [Space]
        [Header("Movement Data")]
        public float AllowPlayerMovement = 0.05f;
        public float DesireRotationSpeed = 0.1f;
        public float MoveSpeed;
        public float JumpForce;

        [Space]
        [Header("Damage Layer")]
        public LayerMask DamageLayer;
    }
}