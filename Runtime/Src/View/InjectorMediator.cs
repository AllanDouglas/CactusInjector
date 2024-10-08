using UnityEngine;

namespace AllanDouglas.CactusInjector
{
    public abstract class InjectorMediator : MonoBehaviour
    {
        [SerializeField] protected Component _view;
    }

    public abstract class InjectedBehaviour : MonoBehaviour { }
}