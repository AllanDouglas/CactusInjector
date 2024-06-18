using UnityEngine;

namespace AllanDouglas.CactusInjector.Editor
{
    public sealed class CactusInjectorConfigSO : ScriptableObject
    {
        [SerializeField] private CactusInjectorContainerSO _container;

        public CactusInjectorContainerSO Container { get => _container; set => _container = value; }
    }
}
