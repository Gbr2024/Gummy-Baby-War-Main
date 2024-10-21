using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMaker : MonoBehaviour
{
    public GameObject cloudSpherePrefab;  // The sphere prefab to generate clouds
    public int minSpheres = 20;           // Minimum number of spheres per cloud
    public int maxSpheres = 50;           // Maximum number of spheres per cloud
    public Vector3 cloudGroupSize = new Vector3(5, 3, 5);  // Tightened size of each cloud group area
    public float minScale = 3f;           // Minimum scale for spheres (increased by 3x)
    public float maxScale = 9f;           // Maximum scale for spheres (increased by 3x)
    public int numberOfClouds = 100;      // Number of clouds to generate
    public Vector2 sceneSize = new Vector2(750, 750);  // Size of the sky area to fill
    public float skyHeight = 50;          // Height at which clouds will be placed
    public Transform center;              // Central position for cloud generation

    void Start()
    {
        GenerateClouds();
    }

    void GenerateClouds()
    {
        for (int i = 0; i < numberOfClouds; i++)
        {
            CreateCloudGroup();
        }
    }

    void CreateCloudGroup()
    {
        // Create a new empty game object to hold the cloud group
        GameObject cloudGroup = new GameObject("CloudGroup");

        // Set a random position for the entire cloud group relative to the center transform
        cloudGroup.transform.position = new Vector3(
            center.position.x + Random.Range(-sceneSize.x / 2, sceneSize.x / 2), // Cloud group spread relative to center
            center.position.y + skyHeight, // Clouds placed at the desired height above the center
            center.position.z + Random.Range(-sceneSize.y / 2, sceneSize.y / 2)
        );

        // Determine the number of spheres for this cloud group
        int numberOfSpheres = Random.Range(minSpheres, maxSpheres);

        // Generate spheres for the cloud group, ensuring they are tightly packed
        for (int i = 0; i < numberOfSpheres; i++)
        {
            GameObject sphere = Instantiate(cloudSpherePrefab, cloudGroup.transform);

            // Set a random position within a very tight cluster to wrap the spheres into one cohesive cloud
            sphere.transform.localPosition = new Vector3(
                Random.Range(-cloudGroupSize.x / 2, cloudGroupSize.x / 2) * 0.5f,   // Tighter X-axis spread
                Random.Range(-cloudGroupSize.y / 2, cloudGroupSize.y / 2) * 0.5f,   // Tighter Y-axis spread (flat clouds)
                Random.Range(-cloudGroupSize.z / 2, cloudGroupSize.z / 2) * 0.5f    // Tighter Z-axis spread
            );

            // Set a larger random scale for the sphere to make them cloud-like
            float randomScale = Random.Range(minScale, maxScale);
            sphere.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        }
    }
}
