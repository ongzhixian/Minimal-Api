namespace Clarus.Extensions;

public class TypeDiscovery
{
    public static IEnumerable<Type> GetImplementationsOf<TInterface>()
    {
        var interfaceType = typeof(TInterface);

        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => interfaceType.IsAssignableFrom(type)
                           && type.IsClass
                           && !type.IsAbstract);
    }

}
