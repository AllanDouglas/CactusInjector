using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AllanDouglas.CactusInjector.Editor
{
    public static class Injector
    {
        public const string DI_CONTAINER_DIRECTORY = "Assets/CactusInjector/";
        public const string DI_CONTAINER_FILE_NAME = "DefaultCactusInjectorContainer";


        [UnityEditor.InitializeOnLoadMethod]
        public static void Setup()
        {
            AssemblyReloadEvents.afterAssemblyReload += Inject;
        }

        [MenuItem("Cactus Injector/Inject")]
        public static void Inject()
        {
            Debug.Log("Execute");
            string filePath = Path.Combine(DI_CONTAINER_DIRECTORY, DI_CONTAINER_FILE_NAME + ".asset");
            var diContainer = AssetDatabase.LoadAssetAtPath<CactusInjectorContainerSO>(filePath);

            foreach (var instance in diContainer.GetAllInstance())
            {
                var fieldsWithAttributes = instance.GetType()
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(t => t.GetCustomAttributes(typeof(InjectAttribute), false).Length > 0);

                foreach (var item in fieldsWithAttributes)
                {
                    if (item.GetValue(instance) is null && 
                        ResolveType(diContainer,
                                    item,
                                    item.GetCustomAttribute<InjectAttribute>(),
                                    out var obj))
                    {
                        item.SetValue(instance, obj);
                    }
                }
            }

            static bool ResolveByType(
                CactusInjectorContainerSO diContainer,
                FieldInfo item,
                InjectAttribute attrib,
                out Object obj)
            {
                return diContainer.TryResolve((attrib.TryGetType(out var mapType)
                                                ? mapType
                                                : item.GetType()), out obj);
            }

            static bool ResolveByTagName(
                CactusInjectorContainerSO diContainer,
                FieldInfo item,
                InjectAttribute attrib,
                out Object obj)
            {
                return diContainer.TryResolve((attrib.TryGetTag(out var mapType)
                                                ? mapType
                                                : item.Name), out obj);
            }

            static bool ResolveType(
                CactusInjectorContainerSO diContainer,
                FieldInfo item,
                InjectAttribute attrib,
                out Object obj)
            {
                return (attrib.UsesType
                    ? ResolveByType(diContainer, item, attrib, out obj)
                    : ResolveByTagName(diContainer, item, attrib, out obj));
            }
        }

        [MenuItem("Cactus Injector/Create DI Container")]
        private static void CreateDIContainer()
        {
            if (EditorUtility.DisplayDialog("Attention", "Are you sure?", "Yes", "No"))
            {
                Directory.Delete(DI_CONTAINER_DIRECTORY);
                SafeCreateDIContainer();
            }
        }


        private static void SafeCreateDIContainer()
        {
            if (!Directory.Exists(DI_CONTAINER_DIRECTORY))
            {
                Directory.CreateDirectory(DI_CONTAINER_DIRECTORY);
                string filePath = Path.Combine(DI_CONTAINER_DIRECTORY, DI_CONTAINER_FILE_NAME + ".asset");
                if (!File.Exists(filePath))
                {
                    var container = ScriptableObject.CreateInstance<CactusInjectorContainerSO>();

                    AssetDatabase.CreateAsset(container, filePath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

    }
}
