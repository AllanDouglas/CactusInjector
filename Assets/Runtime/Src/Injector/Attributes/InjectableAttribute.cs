using System;

namespace AllanDouglas.CactusInjector
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class,
        Inherited = false,
        AllowMultiple = false)]
    public sealed class InjectableAttribute : Attribute
    {
        
    }
}
