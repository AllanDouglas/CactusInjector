using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AllanDouglas.CactusInjector
{

    [CreateAssetMenu(menuName = "Cactus Injector/Cactus Injector Container")]
    public sealed class CactusInjectorContainerSO : ScriptableObject
    {
        public const string TagsFilePath = "Assets/CactusInjector/Scripts/Generated/GeneratedInjectableTags.cs";

        [SerializeField] private ScriptableObjectByTypeMappingSet[] _scriptableObjectByTypeMappings;
        [SerializeField] private ScriptableObjectByTagMappingSet[] _scriptableObjectByTagMappings;
        [SerializeField] private ScriptableObject[] _selfInjected;

        public IEnumerable<ScriptableObject> GetAllInstance() => _scriptableObjectByTypeMappings.Select(t => t.Instance).Union(_selfInjected);

        public bool TryResolve<T>(out T obj)
            where T : ScriptableObject
        {

            for (int i = 0; i < _scriptableObjectByTypeMappings.Length; i++)
            {
                if (_scriptableObjectByTypeMappings[i].Type.GetMappedType() == typeof(T))
                {
                    obj = _scriptableObjectByTypeMappings[i].Instance as T;
                    return true;
                }
            }

            obj = default;
            return false;
        }

        public bool TryResolve(Type type, out UnityEngine.Object obj)
        {
            for (int i = 0; i < _scriptableObjectByTypeMappings.Length; i++)
            {
                if (_scriptableObjectByTypeMappings[i].Type.GetMappedType() == type)
                {
                    obj = _scriptableObjectByTypeMappings[i].Instance;
                    return true;
                }
            }

            obj = default;
            return false;
        }

        public bool TryResolve(string tag, out UnityEngine.Object obj)
        {
            for (int i = 0; i < _scriptableObjectByTagMappings.Length; i++)
            {
                if (_scriptableObjectByTagMappings[i].Tag == tag)
                {
                    obj = _scriptableObjectByTagMappings[i].Instance;
                    return true;
                }
            }

            obj = default;
            return false;
        }


        [Serializable]
        private struct ScriptableObjectByTypeMappingSet
        {
            public TypeName Type;
            public ScriptableObject Instance;
        }

        [Serializable]
        private struct ScriptableObjectByTagMappingSet
        {
            public string Tag;
            public ScriptableObject Instance;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_scriptableObjectByTagMappings != null && _scriptableObjectByTagMappings.Length > 0)
            {
                GenerateInjectableTags.GenerateFile(_scriptableObjectByTagMappings
                    .Where(t => !string.IsNullOrEmpty(t.Tag) && t.Instance != null)
                    .Select(t => t.Tag),
                    TagsFilePath);
            }
        }
#endif

    }
}
