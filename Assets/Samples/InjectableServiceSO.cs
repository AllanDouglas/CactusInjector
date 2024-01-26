using UnityEngine;

[CreateAssetMenu(menuName = "CactusInjector/Tests/InjectableService")]
public sealed class InjectableServiceSO : ScriptableObject, IInjectableService
{
    public string GetName() => name;
}
