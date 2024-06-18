using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AllanDouglas.CactusInjector.Editor
{
    [CustomPropertyDrawer(typeof(TypeName))]
    public class TypeNameDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new();

            SerializedProperty nameProperty = property.FindPropertyRelative("_name");

            
            var injectableTypes = GetInjectableTypes();

            if (injectableTypes.Count > 0)
            {
                var typeDropdown = new PopupField<string>("Type",
                    injectableTypes, 
                    injectableTypes.IndexOf(nameProperty.stringValue));
                
                if (string.IsNullOrEmpty(nameProperty.stringValue))
                {
                    typeDropdown.value = nameProperty.stringValue;
                }

                typeDropdown.RegisterValueChangedCallback(evt =>
                {
                    nameProperty.stringValue = evt.newValue;
                    property.serializedObject.ApplyModifiedProperties();
                });

                container.Add(typeDropdown);

                container.style.height = typeDropdown.resolvedStyle.height;
            }

            return container;
        }

        // Get all types marked with InjectableAttribute
        private List<string> GetInjectableTypes() => TypeUtility.InjectablesTypesNames;
    }
}
