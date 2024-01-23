using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    void Update()
    {
        // Check for left mouse button click
        if (Input.GetMouseButtonDown(0))
        {
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        // Get the center of the screen in world space
        Vector3 mousePosition = Input.mousePosition;

        // Create a ray from the camera through the mouse position
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        // Instantiate the projectile at the camera position
        GameObject projectile = Instantiate(projectilePrefab, Camera.main.transform.position, Quaternion.identity);
        projectile.SetActive(true);
        // Calculate the direction based on the ray
        Vector3 direction = ray.direction;

        // Set the initial velocity of the projectile
        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
    }
}
