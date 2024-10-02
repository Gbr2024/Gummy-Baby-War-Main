using UnityEngine;
using UnityEditor;

public class SnapToTerrain : EditorWindow
{
    [MenuItem("Tools/Snap Objects to Terrain")]
    public static void ShowWindow()
    {
        GetWindow<SnapToTerrain>("Snap to Terrain");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Snap Selected Objects to Terrain"))
        {
            SnapSelectedObjectsToTerrain();
        }
    }

    private static void SnapSelectedObjectsToTerrain()
    {
        if (Terrain.activeTerrain == null)
        {
            Debug.LogError("No active terrain found in the scene.");
            return;
        }

        Terrain terrain = Terrain.activeTerrain;
        foreach (GameObject obj in Selection.gameObjects)
        {
            RaycastHit hit;
            // Perform a raycast from above the object straight down to the terrain
            if (Physics.Raycast(obj.transform.position + Vector3.up * 1000, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
            {
                obj.transform.position = new Vector3(
                    obj.transform.position.x,
                    hit.point.y,
                    obj.transform.position.z
                );
            }
            else
            {
                Debug.LogWarning($"Could not snap {obj.name} as no terrain was found below it.");
            }
        }
    }
}
