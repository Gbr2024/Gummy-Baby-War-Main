using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace WeirdBrothers.CharacterController
{
    [InitializeOnLoad]
    public class WBCharacterControllerIcon
    {
        static WBCharacterControllerIcon()
        {
            EditorApplication.hierarchyWindowItemOnGUI += CharacterControllerIcon;
        }
        static void CharacterControllerIcon(int instanceId, Rect selectionRect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (go == null) return;

            var tpCamera = go.GetComponent<WBCharacterController>();
            if (tpCamera != null) DrawIcon("WB-Icon", selectionRect);
        } 

        private static void DrawIcon(string texName, Rect rect)
        {
            Rect r = new Rect(rect.x + rect.width - 16f, rect.y, 16f, 16f);
            //GUI.DrawTexture(r, GetTex(texName));
        }

        private static Texture2D GetTex(string name)
        {
            return (Texture2D)Resources.Load(name);
        }
    }
}
