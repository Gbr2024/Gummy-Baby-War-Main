using UnityEngine;
using Cinemachine;
using WeirdBrothers.ThirdPersonController;

public class SmoothFollow : MonoBehaviour
{
    Transform Cam; // Target transform to follow
    [SerializeField] float smoothTime=.1f; // Speed of following
    //Vector3 targetPosition, newPosition, veclocity=Vector3.zero;


    private void Update()
    {
        if (Cam == null)
            return;

        // Get the current position of the camera

      
        // Get the target position

        // Update the position of the camera
        Cam.position = transform.position;
        Cam.forward = transform.forward;
    }

    internal void SetCam(Transform came)
    {
        Cam = came;
    }


}
