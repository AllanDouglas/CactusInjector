using AllanDouglas.CactusInjector;
using UnityEngine;

[CreateAssetMenu(menuName = "CactusInjector/Tests/InjectableService2")]
public sealed class InjectableService2SO : ScriptableObject, IInjectableService2
{
    [SerializeField, Inject(typeof(IInjectableService))] InjectableServiceSO _injectedService;

    public string GetName() => name;
}