using Microsoft.CodeAnalysis;

namespace Clarus.EndpointSourceGenerator;

[Generator]
public class HelloWorldSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // This is where you would register your code generation logic.
        // For a simple example, we'll just add a static source.
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("HelloWorldGenerated.g.cs", @"
namespace Clarus.EndpointSourceGenerator
{
    public static class HelloWorld
    {
        public static string SayHello()
        {
            return ""Hello, World! I was generated incrementally."";
        }
    }
}");
        });

    } // END METHOD

} // END CLASS
