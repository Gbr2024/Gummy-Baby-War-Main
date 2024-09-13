using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Unity.Netcode;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WeirdBrothers.CharacterController
{
    public class WBCharacterController : MonoBehaviour
    {
        #region Collider        
        [SerializeField]
        private CapsuleColliderData _colliderData;
        private CapsuleCollider _oldCollider;
        private List<CapsuleCollider> _capsuleColliders = new List<CapsuleCollider>();

        public CapsuleCollider Collider
        {
            get
            {
                if (_capsuleColliders.Count <= 0)
                {
                    GetCollider();
                }
                else if (_oldCollider != null)
                {
                    if (!_oldCollider.CompareCollider(_colliderData))
                    {

                        UpdateColliderData();
                    }
                }

                return _capsuleColliders[0];
            }
        }

        private void GetCollider()
        {
            _capsuleColliders.Clear();

            _capsuleColliders = gameObject.GetComponents<CapsuleCollider>().ToList();

            if (_capsuleColliders.Count == 0)
            {
                CapsuleCollider collider = gameObject.AddComponent<CapsuleCollider>();
                _capsuleColliders.Add(collider);
            }

            _capsuleColliders[0].hideFlags = HideFlags.HideInInspector;
            UpdateColliderValues();
        }

        private void UpdateColliderData()
        {
            if (!Application.isPlaying) return;
            _oldCollider.SetCapsuleColliderData(_colliderData);
        }

        private void UpdateColliderValues()
        {
            if (!Application.isPlaying) return;
            Collider.SetCapsuleColliderData(_colliderData);

            if (_capsuleColliders.Count > 0)
                _oldCollider = _capsuleColliders[0];
        }
        #endregion

        #region RigidBody
        [SerializeField]
        private RigidBodyData _rigidBodyData;
        private Rigidbody _oldRigidBody;
        private List<Rigidbody> _rigidBodies = new List<Rigidbody>();

        public Rigidbody Rigidbody
        {
            get
            {
                if (_rigidBodies.Count <= 0)
                {
                    GetRigidBody();
                }
                else if (_oldRigidBody != null)
                {
                    if (!_oldRigidBody.CompareRigidBody(_rigidBodyData))
                    {
                        UpdateRigidBodyData();
                    }
                }

                return _rigidBodies[0];
            }
        }

        

        private void GetRigidBody()
        {
            _rigidBodies.Clear();

            _rigidBodies = gameObject.GetComponents<Rigidbody>().ToList();

            if (_rigidBodies.Count == 0)
            {
                Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                _rigidBodies.Add(rb);
            }

            _rigidBodies[0].hideFlags = HideFlags.HideInInspector;
            UpdateRigidBodyValues();
        }

        private void UpdateRigidBodyData()
        {
            if (!Application.isPlaying) return;
            _oldRigidBody.SetRigidBodyData(_rigidBodyData);
        }

        private void UpdateRigidBodyValues()
        {
            if (!Application.isPlaying) return;
            Rigidbody.SetRigidBodyData(_rigidBodyData);

            if (_rigidBodies.Count > 0)
                _oldRigidBody = _rigidBodies[0];
        }

        #endregion

        #region GroundChecker

        [SerializeField]
        private GroundCheckerData _groundCheckerData;

        private bool _isGrounded;
        private float _groundDistance;

        

        private RaycastHit _hit;
        public bool IsGrounded => _isGrounded;
        public float GroundDistance => _groundDistance;

        private void CheckForGrounded()
        {
            if (_groundCheckerData.GroundChecker == null)
            {
                GameObject groundChecker = new GameObject();
                groundChecker.transform.position = transform.position;
                groundChecker.transform.SetParent(transform);
                groundChecker.name = "GroundChecker";
            }

            bool hasDetectedHit = Physics.CheckSphere(_groundCheckerData.GroundChecker.position, _groundCheckerData.Radius, _groundCheckerData.Layer);

            if (!hasDetectedHit)
            {
                _isGrounded = false;
            }
            else
            {
                _isGrounded = true;
            }

            if (_isGrounded) return;

            if (Physics.Raycast(_groundCheckerData.GroundChecker.position, -_groundCheckerData.GroundChecker.up, out _hit, 100, _groundCheckerData.Layer))
            {
                _groundDistance = _hit.distance;
            }
            else
            {
                _groundDistance = 100f;
            }
        }

        #endregion

        #region Move

        private Vector3 _moveVector = Vector3.zero;

        public void Move(Vector3 moveVector)
        {
            _moveVector = moveVector;
        }

        public void Jump(float force)
        {
            //Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, force, Rigidbody.velocity.z);
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
            Rigidbody.AddForce(Vector3.up * force, ForceMode.VelocityChange);
        }

        #endregion

        private void Awake()
        {
           
            if (_colliderData.EnableCollider)
                GetCollider();
            GetRigidBody();
        }

        

        private void FixedUpdate()
        {
            if (_colliderData.EnableCollider)
            {
                GetCollider();
            }

            CheckForGrounded();
            if (_moveVector.magnitude > 0.1f)
            {
                Rigidbody.velocity = new Vector3(0,Rigidbody.velocity.y,0);
                Rigidbody.angularVelocity = new Vector3(0,Rigidbody.angularVelocity.y,0);
                //Rigidbody.velocity = _moveVector;
                Rigidbody.velocity = new Vector3(_moveVector.x, Rigidbody.velocity.y, _moveVector.z); 
                //Rigidbody.MovePosition(Rigidbody.position + _moveVector * Time.fixedDeltaTime);
            }
        }

        public void SetData(CapsuleColliderData capsuleData)
        {
            _colliderData = capsuleData;
        }

        public void SetData(CapsuleColliderData capsuleData, RigidBodyData rigidBodyData, GroundCheckerData groundCheckerData)
        {
            _colliderData = capsuleData;
            _rigidBodyData = rigidBodyData;
            _groundCheckerData = groundCheckerData;
        }

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && _colliderData.EnableCollider)
            {
                DrawWireCapsule(transform.position, transform.rotation,
                                _colliderData.Radius, _colliderData.Height,
                                _colliderData.Center, Color.green);
            }
#endif
        }

        public static void DrawWireCapsule(Vector3 pos, Quaternion rot, float radius, float height, Vector3 center,
           Color color = default)
        {
#if UNITY_EDITOR
            if (color != default)
            {
                Handles.color = color;
            }
            pos += center;

            Matrix4x4 angleMatrix = Matrix4x4.TRS(pos, rot, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                float pointOffset = (height - (radius * 2)) / 2;

                // Draw sideways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, radius);
                Handles.DrawLine(new Vector3(0, pointOffset, -radius), new Vector3(0, -pointOffset, -radius));
                Handles.DrawLine(new Vector3(0, pointOffset, radius), new Vector3(0, -pointOffset, radius));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, radius);
                // Draw front
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, radius);
                Handles.DrawLine(new Vector3(-radius, pointOffset, 0), new Vector3(-radius, -pointOffset, 0));
                Handles.DrawLine(new Vector3(radius, pointOffset, 0), new Vector3(radius, -pointOffset, 0));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, radius);
                // Draw center
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, radius);
            }
#endif
        }
    }
}
