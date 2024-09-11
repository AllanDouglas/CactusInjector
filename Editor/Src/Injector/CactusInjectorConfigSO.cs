using AllanDouglas.CactusInjector.Attributes;
using UnityEngine;

namespace AllanDouglas.CactusInjector.Editor
{
    public sealed class CactusInjectorConfigSO : ScriptableObject
    {
        [SerializeField, CreateScriptable] private CactusInjectorContainerSO[] _container;

        public CactusInjectorContainerSO[] Containers { get => _container; set => _container = value; }
    }
}
