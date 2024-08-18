using System.IO;
using UnityEditor;
using UnityEngine;

namespace AllanDouglas.CactusInjector.Editor
{
    public static partial class Injector
    {
        public const string DI_CONFIG_CONTAINER_DIRECTORY = "Assets/CactusInjector/Editor";
        public const string DI_CONFIG_CONTAINER_FILE_NAME = "DefaultCactusInjectorConfig";
        private const string DI_CONTAINER_DIRECTORY = "Assets/CactusInjector";
        private const string DI_CONTAINER_NAME = "CactusInjectorContainer.asset";

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