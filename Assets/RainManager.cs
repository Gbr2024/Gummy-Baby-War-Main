using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainManager : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] Color GrayMat;
    [SerializeField] Light[] Thunder;
    [SerializeField] float minThunderDelay = 5.0f;  // Minimum delay between lightning strikes
    [SerializeField] float maxThunderDelay = 15.0f; // Maximum delay between lightning strikes
    [SerializeField] float flashDuration = 0.2f;    // Duration of each lightning flash
    [SerializeField] float maxLightIntensity = 3.0f; // Max intensity of lightning flash

    private Material objectMaterial;

    // Start is called before the first frame update
    void Start()
    {
        ChangeColorAndIntensity();
        StartCoroutine(LightningRoutine());
    }

    void ChangeColorAndIntensity()
    {
        // Modify the material color
        material.SetColor("_BaseColor", GrayMat);

        // Disable emission if not needed
        material.DisableKeyword("_EMISSION");
    }

    IEnumerator LightningRoutine()
    {
        while (true)
        {
            // Wait for a random delay before next lightning strike
            float delay = Random.Range(minThunderDelay, maxThunderDelay);
            yield return new WaitForSeconds(delay);

            // Flash the lightning a few times to mimic lightning flickers
            int flashCount = Random.Range(1, 4); // Random number of flashes
            for (int i = 0; i < flashCount; i++)
            {
                // Randomize light intensity for each flash
                foreach (var item in Thunder)
                {
                    item.intensity = Random.Range(maxLightIntensity / 2, maxLightIntensity);
                }
                

                // Turn the light on for a short duration (flash effect)
                yield return new WaitForSeconds(flashDuration);

                // Turn the light off between flashes
                foreach (var item in Thunder)
                {
                    item.intensity = 0;
                }
                yield return new WaitForSeconds(flashDuration / 2);  // Shorter pause between flashes
            }
        }
    }
}
