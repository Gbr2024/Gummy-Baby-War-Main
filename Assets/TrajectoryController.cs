using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrajectoryController : MonoBehaviour
{
    [SerializeField] Transform startPoint; // Start point transform
    [SerializeField] Transform endPoint;   // End point transform
    [SerializeField] float ThrowDistance = 0.1f;   // Distance of the throw (percentage of total distance)
    [SerializeField] float ThrowForce = 10f;      // Force of the throw

    public int pointCount = 20;  // Number of points on the curve
    public Grenade prefab;         // Rigidbody to be moved along the trajectory
    public Grenade grenade;         // Rigidbody to be moved along the trajectory

    private LineRenderer lineRenderer;
    internal Vector3[] pointsOnCurve;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        pointsOnCurve = new Vector3[pointCount];
    }

    void Update()
    {
        if(lineRenderer.enabled)
        {
            CalculateEndPose();
            CalculateCurvePoints();
            DrawCurve();
        }
      
    }

    private void CalculateEndPose()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;

        // Define min and max z positions
        float minZ = 5f;
        float maxZ = 65f;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit,maxZ))
        {
            // Get the hit point and clamp it within the z constraints
            Vector3 localHitPoint = transform.InverseTransformPoint(hit.point);
            float zPos = Mathf.Clamp(localHitPoint.z, minZ, maxZ);

            // Update the endpoint position only along the local z-axis
            endPoint.localPosition = new Vector3(localHitPoint.x, localHitPoint.y, zPos);
        }
        else
        {
            endPoint.localPosition = new Vector3(0, 0, maxZ);
        }
        //endPoint.localPosition = new Vector3(endPoint.localPosition.x, endPoint.localPosition.y, z);
    }

    float totalDistance;
    void CalculateCurvePoints()
    {
        totalDistance = Vector3.Distance(startPoint.position, endPoint.position);

        // Round the local Z position of the endpoint to the nearest integer
        int roundedZ = Mathf.RoundToInt(endPoint.localPosition.z);
        // Ensure the rounded value is within the range of pointCount
        roundedZ = Mathf.Clamp(roundedZ, 0, pointCount);

        // Calculate the adjusted endpoint position based on the rounded Z position
        Vector3 adjustedEndPoint = endPoint.position + (endPoint.forward.normalized * roundedZ);

        for (int i = 0; i < pointCount; i++)
        {
            float t = i / (float)(pointCount - 1);

            // Calculate the points on the curve with the adjusted endpoint
            pointsOnCurve[i] = CalculateBezierPoint(startPoint.position, adjustedEndPoint, startPoint.forward.normalized, -endPoint.forward.normalized, t, totalDistance);
        }
    }

    void DrawCurve()
    {
        lineRenderer.positionCount = pointCount;
        lineRenderer.SetPositions(pointsOnCurve);
    }

    Vector3 p1;
    Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p2, Vector3 startForward, Vector3 endForward, float t, float totalDistance)
    {
        Vector3 p1 = Vector3.Lerp(p0 + startForward * totalDistance * 0.33f, p2 + endForward * totalDistance * 0.33f, t);
        float curveHeight = Mathf.Sin(t * Mathf.PI) * totalDistance * 0.1f; // Adjust multiplier as needed
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * p0 + 2f * oneMinusT * t * p1 + t * t * p2 + Vector3.up * curveHeight;
    }


    Vector3 startPosition;
    Vector3 endPosition;

    float distance;
    float duration;

    float startTime;
    float elapsedTime = 0f;
    float t;
    float y;


    List<Vector3> points = new();
    IEnumerator MoveObjectAlongTrajectory()
    {
        grenade.ToFollow = null;
        yield return new WaitForSeconds(.15f);
        grenade.hasCollided = false;
        grenade.hasThrown = true;
        grenade.ToFollow = null;
        
        for (int i = 0; i < points.Count-1; i++)
        {
            if (grenade == null) yield break;
            if (i == 3)
            {
                //grenade.rb.interpolation = RigidbodyInterpolation.Interpolate;
                grenade.rb.detectCollisions = true;
                grenade.rb.isKinematic = false;
            }
             startPosition = points[i];
             endPosition = points[i + 1];

             distance = Vector3.Distance(startPosition, endPosition);
             duration = distance / ThrowForce;

             startTime = Time.time;
             elapsedTime = 0f;

            while (elapsedTime < duration && !grenade.hasCollided)
            {
                if (grenade == null) yield break;
                t = elapsedTime / duration;
                grenade.transform.position=(Vector3.LerpUnclamped(startPosition, endPosition, t));
                elapsedTime = Time.time - startTime;
                yield return null;
            }
        }
    }

    internal void ThrowObject()
    {
        StartCoroutine(MoveObjectAlongTrajectory());
    }

    internal void CalculatePointsOnCurve()
    {
        points = pointsOnCurve.ToList();
        foreach (var item in points)
        {
            if (item.y > y)
                y = item.y;
        }
    }


    internal void EnableDraw(bool b)
    {
        lineRenderer.enabled = b;

    }
}
