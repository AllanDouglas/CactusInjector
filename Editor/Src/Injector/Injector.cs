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

            if (TryGetDIContainer(out var containers))
            {
                foreach (var container in containers)
                {
                    if (container.TryResolve(out instance))
                    {
                        return true;
                    }
                }
            }

            instance = default;
            return false;
        }

        public static bool TryResolve<T>(string tag, out T instance)
            where T : ScriptableObject
        {

            if (TryGetDIContainer(out var containers))
            {
                foreach (var container in containers)
                {
                    if (container.TryResolve(tag, out var obj) && obj is T tInstance)
                    {
                        instance = tInstance;
                        return true;
                    }
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

            foreach (var container in diContainer)
            {
                foreach (var instance in container.GetAllInstance())
                {
                    InjectFields(instance, diContainer, GetInjectableFields(instance));
                }

            }

        }

        private static void InjectFields(Object instance, CactusInjectorContainerSO[] diContainers, IEnumerable<FieldInfo> fieldsWithAttributes)
        {
            foreach (var item in fieldsWithAttributes)
            {
                foreach (var contianer in diContainers)
                {
                    if (ResolveType(contianer,
                                    item,
                                    item.GetCustomAttribute<InjectAttribute>(),
                                    out var obj))
                    {
                        if (!item.GetValue(instance).Equals(obj))
                        {
                            item.SetValue(instance, obj);
                        }
                    }
                }

            }
        }

        private static void InjectFields(SerializedObject instance, CactusInjectorContainerSO[] diContainers, IEnumerable<FieldInfo> fieldsWithAttributes)
        {
            foreach (var item in fieldsWithAttributes)
            {
                foreach (var container in diContainers)
                {
                    if (ResolveType(container,
                                    item,
                                    item.GetCustomAttribute<InjectAttribute>(),
                                    out var obj))
                    {
                        var property = instance.FindProperty(item.Name);
                        if (property.objectReferenceValue != obj)
                        {
                            property.objectReferenceValue = obj;
                        }
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

        public static bool TryGetDIContainer(out CactusInjectorContainerSO[] diContainer)
        {
            diContainer = null;
            string filePath = Path.Combine(DI_CONFIG_CONTAINER_DIRECTORY, DI_CONFIG_CONTAINER_FILE_NAME + ".asset");

            var cactusInjectorConfig = AssetDatabase.LoadAssetAtPath<CactusInjectorConfigSO>(filePath);

            if (cactusInjectorConfig == null)
            {
                return false;
            }

            if (cactusInjectorConfig.Containers == null)
            {
                return false;
            }

            diContainer = cactusInjectorConfig.Containers;
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
