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
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).First(t => t.FullName == name);
        }
    }
}
