using System;
using System.Collections.Generic;
using System.Linq;
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
            // Create a new VisualElement container
            VisualElement container = new();

            // Get the SerializedProperties for Name and Type fields
            SerializedProperty nameProperty = property.FindPropertyRelative("_name");

            // Get all types marked with InjectableAttribute
            var injectableTypes = GetInjectableTypes();

            // Create a DropdownField with the types

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

                // Add the DropdownField to the container
                container.Add(typeDropdown);

                // Set the height of the container to match the dropdown
                container.style.height = typeDropdown.resolvedStyle.height;
            }

            return container;
        }

        // Get all types marked with InjectableAttribute
        private List<string> GetInjectableTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes()
                    .Where(t => t.GetCustomAttributes(typeof(InjectableAttribute), true).Length > 0))
                    .Select(t => t.FullName)
                .ToList();
        }
    }
}
