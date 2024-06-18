using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AllanDouglas.CactusInjector.Editor
{
    public static class Injector
    {
        public const string DI_CONFIG_CONTAINER_DIRECTORY = "Assets/CactusInjector/Editor";
        public const string DI_CONFIG_CONTAINER_FILE_NAME = "DefaultCactusInjectorConfig";
        private const string DI_CONTAINER_DIRECTORY = "Assets/CactusInjector";
        private const string DI_CONTAINER_NAME = "CactusInjectorContainer.asset";

        [UnityEditor.InitializeOnLoadMethod]
        public static void Setup()
        {
            AssemblyReloadEvents.afterAssemblyReload += Inject;
        }

        [MenuItem("Cactus Injector/Inject")]
        public static void Inject()
        {
            string filePath = Path.Combine(DI_CONFIG_CONTAINER_DIRECTORY, DI_CONFIG_CONTAINER_FILE_NAME + ".asset");

            var cactusInjectorConfig = AssetDatabase.LoadAssetAtPath<CactusInjectorConfigSO>(filePath);

            if (cactusInjectorConfig == null)
            {
                return;
            }

            if(cactusInjectorConfig.Container == null)
            {
                return;
            }

            var diContainer = cactusInjectorConfig.Container;

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
                SafeCreateDIContainer();
            }
        }

        [MenuItem("Cactus Injector/Open Config")]
        private static void OpenConfigDIContainer()
        {
            string filePath = Path.Combine(DI_CONFIG_CONTAINER_DIRECTORY, DI_CONFIG_CONTAINER_FILE_NAME + ".asset");
            var diContainerConfig = AssetDatabase.LoadAssetAtPath<CactusInjectorConfigSO>(filePath);

            if (diContainerConfig != null)
            {
                Selection.objects = new[] { diContainerConfig };
            }
        }


        private static void SafeCreateDIContainer()
        {
            string filePath = Path.Combine(DI_CONFIG_CONTAINER_DIRECTORY, DI_CONFIG_CONTAINER_FILE_NAME + ".asset");

            var diContainerConfig = AssetDatabase.LoadAssetAtPath<CactusInjectorConfigSO>(filePath);

            if (diContainerConfig == null)
            {
                if (!Directory.Exists(DI_CONFIG_CONTAINER_DIRECTORY))
                {
                    Directory.CreateDirectory(DI_CONFIG_CONTAINER_DIRECTORY);
                }

                diContainerConfig = ScriptableObject.CreateInstance<CactusInjectorConfigSO>();

                AssetDatabase.CreateAsset(diContainerConfig, filePath);
                AssetDatabase.SaveAssets();

            }

            if (diContainerConfig.Container == null)
            {
                if (!Directory.Exists(DI_CONTAINER_DIRECTORY))
                {
                    Directory.CreateDirectory(DI_CONTAINER_DIRECTORY);
                }

                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<CactusInjectorContainerSO>(),
                    Path.Combine(DI_CONTAINER_DIRECTORY, DI_CONTAINER_NAME));

                diContainerConfig.Container = AssetDatabase.LoadAssetAtPath<CactusInjectorContainerSO>(Path.Combine(DI_CONTAINER_DIRECTORY, DI_CONTAINER_NAME));
                AssetDatabase.SaveAssets();
            }

        }

    }
}
