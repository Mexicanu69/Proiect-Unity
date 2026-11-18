using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// CA SA FACI UN CAMP READONLY DAR SA SE VADA IN EDITOR PT TESTARE
/// [SerializeField, ReadOnly] private tip nume = whatever;
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}
#endif