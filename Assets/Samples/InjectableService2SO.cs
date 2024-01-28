using AllanDouglas.CactusInjector;
using UnityEngine;

[CreateAssetMenu(menuName = "CactusInjector/Tests/InjectableService2")]
public sealed class InjectableService2SO : ScriptableObject, IInjectableService2
{
    [SerializeField, Inject(InjectTag.TEST_SERVICE_1)] InjectableServiceSO _injectedService;
    [SerializeField, Inject(typeof(IInjectableService))] ScriptableObject _injectedServiceByName;

    public string GetName() => name;
}