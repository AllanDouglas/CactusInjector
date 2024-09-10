using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AllanDouglas.CactusInjector.Editor
{
    public static partial class Injector
    {
        [InitializeOnLoadMethod]
        public static void Setup() => AssemblyReloadEvents.afterAssemblyReload += Inject;

        public static void ResolveMonoBehaviour(MonoBehaviour monoBehaviour)
        {
            var fieldsWithAttributes = GetInjectableFields(monoBehaviour);

            if (fieldsWithAttributes.Count() > 0 && TryGetDIContainer(out var diContainer))
            {
                InjectFields(monoBehaviour, diContainer, fieldsWithAttributes);
            }
        }

        public static void ResolveMonoBehaviour(SerializedObject serializedObject, Type type)
        {
            var fieldsWithAttributes = GetInjectableFields(type);

            if (fieldsWithAttributes.Count() > 0 && TryGetDIContainer(out var diContainer))
            {
                InjectFields(serializedObject, diContainer, fieldsWithAttributes);

            }
        }

        public static bool TryResolve<T>(T type, out T instance)
            where T : ScriptableObject
        {

            if (TryGetDIContainer(out var container))
            {
                return container.TryResolve(out instance);
            }

            instance = default;
            return false;
        }

        public static bool TryResolve<T>(string tag, out T instance)
            where T : ScriptableObject
        {

            if (TryGetDIContainer(out var container))
            {
                if (container.TryResolve(tag, out var obj) && obj is T tInstance)
                {
                    instance = tInstance;
                    return true;
                }
            }

            instance = default;
            return false;
        }

        [MenuItem("Cactus Injector/Inject")]
        public static void Inject()
        {
            if (!TryGetDIContainer(out var diContainer))
            {
                Debug.LogError("DI Container not loaded!");
                return;
            }

            foreach (var instance in diContainer.GetAllInstance())
            {
                InjectFields(instance, diContainer, GetInjectableFields(instance));
            }
        }

        private static void InjectFields(Object instance, CactusInjectorContainerSO diContainer, IEnumerable<FieldInfo> fieldsWithAttributes)
        {
            foreach (var item in fieldsWithAttributes)
            {
                if (ResolveType(diContainer,
                                item,
                                item.GetCustomAttribute<InjectAttribute>(),
                                out var obj))
                {
                    item.SetValue(instance, obj);
                }
            }
        }

        private static void InjectFields(SerializedObject instance, CactusInjectorContainerSO diContainer, IEnumerable<FieldInfo> fieldsWithAttributes)
        {
            foreach (var item in fieldsWithAttributes)
            {
                if (ResolveType(diContainer,
                                item,
                                item.GetCustomAttribute<InjectAttribute>(),
                                out var obj))
                {
                    var property = instance.FindProperty(item.Name);
                    if (property.objectReferenceValue == null)
                    {
                        property.objectReferenceValue = obj;
                    }
                }
            }
        }

        private static IEnumerable<FieldInfo> GetInjectableFields(Object monoBehaviour) => monoBehaviour.GetType()
                   .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                   .Where(t => t.GetCustomAttributes(typeof(InjectAttribute), inherit: false).Length > 0);
        private static IEnumerable<FieldInfo> GetInjectableFields(Type type) => type
                   .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                   .Where(t => t.GetCustomAttributes(typeof(InjectAttribute), inherit: false).Length > 0);

        public static bool TryGetDIContainer(out CactusInjectorContainerSO diContainer)
        {
            diContainer = null;
            string filePath = Path.Combine(DI_CONFIG_CONTAINER_DIRECTORY, DI_CONFIG_CONTAINER_FILE_NAME + ".asset");

            var cactusInjectorConfig = AssetDatabase.LoadAssetAtPath<CactusInjectorConfigSO>(filePath);

            if (cactusInjectorConfig == null)
            {
                return false;
            }

            if (cactusInjectorConfig.Container == null)
            {
                return false;
            }

            diContainer = cactusInjectorConfig.Container;
            return true;
        }

        static bool ResolveByType(
                CactusInjectorContainerSO diContainer,
                FieldInfo item,
                InjectAttribute attrib,
                out Object obj)
        {
            return diContainer.TryResolve((attrib.TryGetType(out var mapType)
                                            ? mapType
                                            : item.FieldType), out obj);
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
}
