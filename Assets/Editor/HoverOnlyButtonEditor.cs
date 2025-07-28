#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(HoverOnlyButtonMulti))]
public class HoverOnlyButtonMultiEditor : UnityEditor.UI.ButtonEditor
{
    SerializedProperty isP1Prop;
    SerializedProperty targetEventSystemProp;

    protected override void OnEnable()
    {
        base.OnEnable();
        isP1Prop = serializedObject.FindProperty("isP1");
        targetEventSystemProp = serializedObject.FindProperty("targetEventSystem");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Multi-EventSystem Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(isP1Prop);
        EditorGUILayout.PropertyField(targetEventSystemProp);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

