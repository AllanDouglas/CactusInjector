using UnityEditor;

namespace AllanDouglas.CactusInjector.Editor
{
    [CustomEditor(typeof(InjectorMediator), true)]
    public sealed class ViewMediatorEditor : UnityEditor.Editor
    {
        void OnEnable()
        {
            Injector.ResolveMonoBehaviour(serializedObject, target.GetType());
            serializedObject.ApplyModifiedProperties();
            CaptureView();
        }

        private void CaptureView()
        {
            var type = target.GetType();
            var baseType = type.BaseType;
            var genericsTypes = baseType.GetGenericArguments();

            if (genericsTypes.Length > 0)
            {
                var viewType = genericsTypes[0];
                var mediator = (target as InjectorMediator);
                if (mediator.TryGetComponent(viewType, out var component))
                {
                    var viewProperty = serializedObject.FindProperty("_view");
                    viewProperty.objectReferenceValue = component;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }

}
