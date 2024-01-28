using System;
using System.Linq;
using UnityEngine;

namespace AllanDouglas.CactusInjector
{
    [Serializable]
    public struct TypeName
    {
        [SerializeField] private string _name;

        public readonly string Name => _name;

        public readonly Type GetMappedType()
        {
            var name = _name;
            return TypeUtility.Types.First(t => t.FullName == name);
        }
    }
}
