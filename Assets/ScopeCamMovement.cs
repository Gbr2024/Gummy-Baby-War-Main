using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScopeCamMovement : MonoBehaviour
{
    public Transform scopeCamera;
    public float smoothingFactor = 0.1f;

    private Vector3 previousPosition;

    void Start()
    {
        // Initialize the previous position with the current position
        previousPosition = scopeCamera.localPosition;
    }

    void LateUpdate()
    {
        // Calculate the current position of the scope camera
        Vector3 currentPosition = scopeCamera.localPosition;

        // Calculate the difference between the current and previous positions
        Vector3 positionDifference = currentPosition - previousPosition;

        // Apply smoothing to the position difference
        Vector3 smoothedDifference = Vector3.Lerp(Vector3.zero, positionDifference, smoothingFactor);

        // Update the position of the scope camera using the smoothed difference
        scopeCamera.localPosition += smoothedDifference;

        // Update the previous position for the next frame
        previousPosition = currentPosition;
    }
    //[SerializeField] Transform target;

    //float speed = 20f;
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if(target!=null)
    //    {
    //        transform.forward = target.forward;
    //        transform.position = new Vector3(target.position.x,transform.position.y,target.position.z);
    //    }
    //}

    //internal void SetTarget(Transform t)
    //{
    //    target = t;
    //}
}
