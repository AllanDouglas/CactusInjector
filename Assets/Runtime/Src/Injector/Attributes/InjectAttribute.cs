using System;

namespace AllanDouglas.CactusInjector
{
    [AttributeUsage(AttributeTargets.Field,
        Inherited = false,
        AllowMultiple = false)]
    public sealed class InjectAttribute : Attribute
    {
        readonly Type _type;

        public InjectAttribute() { }

        public InjectAttribute(Type type) => _type = type;

        public bool TryGetType(out Type type)
        {
            type = _type;

            return _type != default;
        }
    }
}
