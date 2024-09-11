using AllanDouglas.CactusInjector.Attributes;
using UnityEditor;
using UnityEngine;

namespace AllanDouglas.CactusInjector.Editor
{

    [CustomPropertyDrawer(typeof(CreateScriptableAttribute))]
    public class CreateScriptableDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CreateScriptableAttribute attribute = (CreateScriptableAttribute)base.attribute;

            var type = attribute.Type != null
                ? attribute.Type
                : fieldInfo.FieldType.IsArray
                    ? fieldInfo.FieldType.GetElementType()
                    : fieldInfo.FieldType;

            EditorGUI.BeginProperty(position, label, property);

            position.width -= EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(position, property, label, true);

            if (!type.IsSubclassOf(typeof(ScriptableObject)))
            {
                EditorGUI.EndProperty();
                return;
            }

            Rect buttonRect = position;
            buttonRect.width = EditorGUIUtility.singleLineHeight;
            buttonRect.x += position.width;
            position.xMin += EditorGUIUtility.singleLineHeight + 20f;

            if (GUI.Button(buttonRect, "+"))
            {

                string savePath = EditorUtility.SaveFilePanelInProject(
                    "Save ScriptableObject",
                    type.Name,
                    "asset",
                    "Choose a file location to save the ScriptableObject",
                    attribute.DefaultPath ?? "old");

                if (!string.IsNullOrEmpty(savePath))
                {

                    ScriptableObject newObj = ScriptableObject.CreateInstance(type);
                    AssetDatabase.CreateAsset(newObj, savePath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    property.objectReferenceValue = AssetDatabase.LoadAssetAtPath(savePath, type);
                    property.serializedObject.ApplyModifiedProperties();
                }
                GUIUtility.ExitGUI();
            }

            EditorGUI.EndProperty();
        }
    }
}
