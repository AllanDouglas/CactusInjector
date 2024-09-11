using System;
using UnityEngine;

namespace AllanDouglas.CactusInjector.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class CreateScriptableAttribute : PropertyAttribute
    {
        public CreateScriptableAttribute(Type type = null, string defaultPath = null)
        {
            Type = type;
            DefaultPath = defaultPath;
        }

        public Type Type { get; }

        public string DefaultPath { get; }
    }


}