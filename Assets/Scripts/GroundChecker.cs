using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeirdBrothers.CharacterController;

public class GroundChecker : MonoBehaviour
{
    #region GroundChecker

    [SerializeField] GroundCheckerData _groundCheckerData;
    [SerializeField] Animator animator;
    [SerializeField] bool DebugMessage = false;
    private bool _isGrounded;
    private float _groundDistance;
    private RaycastHit _hit;

    public bool IsGrounded => _isGrounded;
    public float GroundDistance => _groundDistance;

    private void Start()
    {

    }
    private void CheckForGrounded()
    {
        if (_groundCheckerData.GroundChecker == null)
        {
            GameObject groundChecker = new();
            groundChecker.transform.position = transform.position;
            groundChecker.transform.SetParent(transform);
            groundChecker.name = "GroundChecker";
        }

        _isGrounded = Physics.CheckSphere(_groundCheckerData.GroundChecker.position, _groundCheckerData.Radius, _groundCheckerData.Layer);
        if(DebugMessage)Debug.LogError(IsGrounded);
        
        animator.SetBool("IsGrounded", _isGrounded);
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

    // Other code...

    private void FixedUpdate()
    {
        CheckForGrounded();

        // Other FixedUpdate logic...
    }
}

