using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AllanDouglas.CactusInjector.Editor
{
    [CustomEditor(typeof(CactusInjectorContainerSO))]
    public sealed class CactusInjectorContainerEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new();

            var scriptableObjectByTypeMappingsProperty = serializedObject.FindProperty("_scriptableObjectByTypeMappings");
            var scriptableObjectByTagMappingsProperty = serializedObject.FindProperty("_scriptableObjectByTagMappings");

            container.Add(new PropertyField(scriptableObjectByTypeMappingsProperty));
            container.Add(new PropertyField(scriptableObjectByTagMappingsProperty));

            serializedObject.ApplyModifiedProperties();

            return container;
        }
    }
}
