namespace Clarus.Endpoints;


public interface IApiDefinition
{
    static void DoSomething()
    {

    }
}

public static class IApiDefinitionExtensions
{
    public static void GetImplementTypes(this IApiDefinition apiDefinition)
    {

    }
}

//public static class TypeExtensions
//{
//    public static void GetImplementing(this interface t)
//    {

//    }
//}

//public static class ApiDefinitionExtensions
//{
//    public static IEnumerable<Type> GetTypesImplementing<TInterface>(this TInterface _)
//    {
//        var interfaceType = typeof(TInterface);

//        return AppDomain.CurrentDomain.GetAssemblies()
//            .SelectMany(a => a.GetTypes())
//            .Where(t => interfaceType.IsAssignableFrom(t)
//                        && t.IsClass
//                        && !t.IsAbstract);
//    }
//}





public class StandardApi : IApiDefinition
{
    public void DoSomething()
    {

        //IApiDefinition apiDefinition;
        //apiDefinition.GetImplementTypes();

        //IApiDefinition.GetTypesImplementing<IApiDefinition>();
        //var types = ApiDefinitionExtensions.GetTypesImplementing<IApiDefinition>();

        GetTypes.Implementing<IApiDefinition>();

        

    }
}
