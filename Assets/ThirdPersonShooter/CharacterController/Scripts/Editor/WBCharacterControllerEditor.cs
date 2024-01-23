using UnityEditor;

namespace WeirdBrothers.CharacterController
{
    [CustomEditor(typeof(WBCharacterController), true)]
    public class WBCharacterControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
