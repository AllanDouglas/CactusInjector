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

            SerializedProperty scriptableObjectByTypeMappingsProperty = serializedObject.FindProperty("_scriptableObjectByTypeMappings");

            container.Add(new PropertyField(scriptableObjectByTypeMappingsProperty));

            return container;
        }
    }
}
