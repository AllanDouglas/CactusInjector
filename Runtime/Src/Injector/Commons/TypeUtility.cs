using System;
using System.Collections.Generic;
using System.Linq;

namespace AllanDouglas.CactusInjector
{
    public static class TypeUtility
    {
        static Type[] _cacheTypes;
        static List<string> _injectableTypesNames;

        public static IEnumerable<Type> Types
        {
            get
            {
                _cacheTypes ??= AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).ToArray();
                return _cacheTypes;
            }
        }

        public static List<string> InjectablesTypesNames
        {
            get
            {
                _injectableTypesNames ??= Types
                   .Where(t => t.GetCustomAttributes(typeof(InjectableAttribute), true).Length > 0)
                   .Select(t => t.FullName).ToList();

                return _injectableTypesNames;
                
            }
        }
    }
}
