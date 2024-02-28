using UnityEngine;
using Cinemachine;

public class SmoothFollow : MonoBehaviour
{
    Transform Cam; // Target transform to follow
    [SerializeField] float followSpeed = 5f,smoothTime=.1f; // Speed of following
    Vector3 targetPosition, newPosition, veclocity=Vector3.zero;

    private void Update()
    {
        if (Cam == null)
            return;

        // Get the current position of the camera

        // Get the target position
         targetPosition = transform.position;

        // Calculate the new position for the camera
         newPosition = Vector3.SmoothDamp(Cam.position, targetPosition, ref veclocity, smoothTime, followSpeed);

        // Update the position of the camera
        Cam.position = newPosition;
        Cam.forward = transform.forward;
    }

    internal void SetCam(Transform came)
    {
        Cam = came;
    }
}
