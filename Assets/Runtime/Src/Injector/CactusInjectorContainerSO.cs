using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace AllanDouglas.CactusInjector
{

    [CreateAssetMenu(menuName = "Cactus Injector/Cactus Injector Container")]
    public sealed class CactusInjectorContainerSO : ScriptableObject
    {
        [SerializeField]
        private ScriptableObjectByTypeMappingSet[] _scriptableObjectByTypeMappings;

        public IEnumerable<ScriptableObject> GetAllInstance() => _scriptableObjectByTypeMappings.Select(t => t.Instance);

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


        [Serializable]
        private struct ScriptableObjectByTypeMappingSet
        {
            public TypeName Type;
            public ScriptableObject Instance;
        }

    }
}
