// Type discovery/searching utility? 

using System.ComponentModel;

public static class GetTypes
{
    //GetTypesImplementing<IApiDefinition>();
    //GetTypesWithAttribute<SomeAttribute>();

    //public static IEnumerable<Type> Implementing<TInterface>()
    //{
    //    var interfaceType = typeof(TInterface);

    //    return AppDomain.CurrentDomain.GetAssemblies()
    //        .SelectMany(a => a.GetTypes())
    //        .Where(t => interfaceType.IsAssignableFrom(t)
    //                    && t.IsClass
    //                    && !t.IsAbstract);
    //}

    public static IEnumerable<Type> Implementing<TInterface>(bool interfaceOnlyCheck = false) where TInterface : class
    {
        var interfaceType = typeof(TInterface);

        if (interfaceOnlyCheck && (!interfaceType.IsInterface))
            throw new ArgumentException($"{interfaceType.FullName} is not an interface.");

        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => interfaceType.IsAssignableFrom(t)
                        && t.IsClass
                        && !t.IsAbstract);
    }


    public static IEnumerable<Type> WithAttribute<TAttribute>()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.GetCustomAttributes(typeof(DisplayNameAttribute), inherit: true).Any());
    }
}

