#if UNITY_EDITOR
#endif

using UnityEngine;

namespace AllanDouglas.CactusInjector
{
    public abstract class ViewMediator<TView> : InjectorMediator
        where TView : Component
    {

        private TView _cachedView;

        public TView View
        {
            get
            {
                if (_cachedView is not null)
                {
                    return _cachedView;
                }

                _cachedView = (TView)_view;
                return _cachedView;
            }
            protected set
            {
                _view = value;
                _cachedView = value;
            }
        }

    }

}
