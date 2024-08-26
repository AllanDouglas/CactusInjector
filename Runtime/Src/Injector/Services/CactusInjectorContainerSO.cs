using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AllanDouglas.CactusInjector
{
    [CreateAssetMenu(menuName = "Cactus Injector/Cactus Injector Container")]
    public sealed class CactusInjectorContainerSO : ScriptableObject
    {

        [SerializeField] private ScriptableObjectByTypeMappingSet[] _scriptableObjectByTypeMappings;
        [SerializeField] private ScriptableObjectByTagMappingSet[] _scriptableObjectByTagMappings;
        [SerializeField] private ScriptableObject[] _selfInjected;

        HashSet<ScriptableObject> _instances;

        public IEnumerable<ScriptableObject> GetAllInstance()
        {
            var byTypeMaps = (_scriptableObjectByTypeMappings != null)
                 ? _scriptableObjectByTypeMappings
                 : Array.Empty<ScriptableObjectByTypeMappingSet>();

            var byTagMaps = (_scriptableObjectByTagMappings != null)
                 ? _scriptableObjectByTagMappings
                 : Array.Empty<ScriptableObjectByTagMappingSet>();


            var self = (_selfInjected != null)
                 ? _selfInjected
                 : Array.Empty<ScriptableObject>();

            _instances ??= byTypeMaps
                        .Select(t => t.Instance)
                        .Union(byTagMaps.Select(t => t.Instance))
                        .Union(self).ToHashSet();

            return _instances;
        }

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

            for (int i = 0; i < _selfInjected.Length; i++)
            {
                if (_selfInjected[i].GetType() == type)
                {
                    obj = _selfInjected[i];
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
                    .Select(t => t.Tag));
            }
        }
#endif

    }
}
