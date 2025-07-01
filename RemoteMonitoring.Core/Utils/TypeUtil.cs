using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

namespace RemoteMonitoring.Core.Utils;

public static class TypeUtil
{
    /// <summary>
    /// Obtain all implementation types
    /// 获得所有实现类型
    /// </summary>
    /// <typeparam name="T">Whose implementation type do we need to obtain?</typeparam>
    /// <returns> All implementation types </returns>
    public static List<Type> ObtainImplementer<T>()
    {
        var implementedBy = typeof(T);
        return ObtainImplementer(implementedBy);
    }
    
    /// <summary>
    /// Obtain all implementation types
    /// 获得所有实现类型
    /// </summary>
    /// <param name="implementedBy">Whose implementation type do we need to obtain?</param>
    /// <returns> All implementation types </returns>
    public static List<Type> ObtainImplementer(Type implementedBy)
    {
        var entityTypes = ObtainTypesThroughDifferentTaggingMethods(implementedBy, ScanType.IsAssignableFrom);
        return entityTypes;
    }
    
    /// <summary>
    /// Obtain the implementation of all generics
    /// </summary>
    /// <typeparam name="T">Need to obtain the generic implementation of it</typeparam>
    /// <returns> All implementation types </returns>
    public static List<Type> ObtainGenericImplementer<T>()
    {
        var implementedBy = typeof(T);
        return ObtainGenericImplementer(implementedBy);
    }
    
    /// <summary>
    /// Obtain the implementation of all generics
    /// </summary>
    /// <param name="genericType">Need to obtain the generic implementation of it</param>
    /// <returns> All implementation types </returns>
    private static List<Type> ObtainGenericImplementer(Type genericType)
    {
        var entityTypes = ObtainTypesThroughDifferentTaggingMethods(genericType, ScanType.GetGenericTypeDefinition);
        return entityTypes;
    }
    
    /// <summary>
    /// Obtain all classes with specified characteristics
    /// </summary>
    /// <typeparam name="T">Types of Attribute tags</typeparam>
    /// <returns> All classes with specified characteristics </returns>
    public static List<Type> ObtainImplementerByAttribute<T>() where T : Attribute
    {
        var entityTypes = ObtainImplementerByAttribute(typeof(T));
        return entityTypes;
    }
    
    /// <summary>
    ///     Obtain all classes with specified characteristics
    /// </summary>
    /// <param name="attributeType"> Types of Attribute tags </param>
    /// <returns> All classes with specified characteristics </returns>
    public static List<Type> ObtainImplementerByAttribute(Type attributeType)
    {
        var entityTypes = ObtainTypesThroughDifferentTaggingMethods(attributeType, ScanType.GetCustomAttribute);
        return entityTypes;
    }

    private static List<Type> ObtainTypesThroughDifferentTaggingMethods(Type type, ScanType scanType)
    { 
        List<Type> entityTypes = [];
        var libs = GetAllProjectTypeLibrary();
        foreach (var lib in libs)
        {
            List<Type> currentTypes = [];
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
            switch (scanType)
            {
                case ScanType.IsAssignableFrom:
                    currentTypes = assembly.GetTypes()
                        .Where(x =>
                            x.GetTypeInfo().BaseType != null
                            && x is { IsAbstract: false, IsClass: true, IsGenericType: false }
                            && type.IsAssignableFrom(x)).ToList();
                    break;
                case ScanType.GetGenericTypeDefinition:
                    currentTypes = assembly.GetTypes()
                        .Where(x =>
                            x is { IsAbstract: false, IsClass: true, IsGenericType: true }
                            && x.GetInterfaces().Any(i =>i.IsGenericType && i.GetGenericTypeDefinition() == type)).ToList();
                    break;
                case ScanType.GetCustomAttribute:
                    currentTypes = assembly.ExportedTypes
                        .Where(x =>
                            x.GetTypeInfo().BaseType != null
                            && x is { IsAbstract: false, IsClass: true, IsGenericType: false }
                            && x.GetCustomAttribute(type) != null).ToList();
                    break;
            }

            if (currentTypes.Any()) entityTypes.AddRange(currentTypes);
        }
        return entityTypes;
    }
    
    /// <summary>
    /// Get all project type assemblies
    /// </summary>
    /// <returns> All project type assemblies </returns>
    public static IEnumerable<CompilationLibrary> GetAllProjectTypeLibrary()
    {
        var libs = DependencyContext.Default!.CompileLibraries
            .Where(x => !x.Serviceable && x.Type != "package" && x.Type == "project");
        return libs;
    }
    
    private enum ScanType
    {
        IsAssignableFrom,
        GetGenericTypeDefinition,
        GetCustomAttribute
    }
}