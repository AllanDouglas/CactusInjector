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
            VisualElement defaultInspector = new ();
            InspectorElement.FillDefaultInspector(defaultInspector, serializedObject, this);
            return defaultInspector;
        }
    }

}
