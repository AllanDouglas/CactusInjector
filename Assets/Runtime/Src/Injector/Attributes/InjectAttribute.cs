using System;

namespace AllanDouglas.CactusInjector
{
    [AttributeUsage(AttributeTargets.Field,
        Inherited = false,
        AllowMultiple = false)]
    public sealed class InjectAttribute : Attribute
    {
        readonly Type _type;
        readonly string _tag;

        public InjectAttribute() { }

        public InjectAttribute(Type type) => _type = type;
        public InjectAttribute(string tag) => _tag = tag;

        public bool UsesType => _type != default;

        public bool TryGetType(out Type type)
        {
            type = _type;

            return _type != default;
        }

        public bool TryGetTag(out string tag)
        {
            tag = _tag;
            return tag != default;
        }
    }
}
