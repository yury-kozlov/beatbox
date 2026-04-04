// Console.SetOut is global state — running test classes in parallel causes race conditions
// where one class's OutputHelperWriter is overwritten by another, then xUnit throws
// "There is no currently active test." when code calls Console.WriteLine after the owning test finishes.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Tests;

/// <summary>
/// Base class for all test classes. Redirects <see cref="Console.WriteLine(string)"/> output
/// to xUnit's <see cref="ITestOutputHelper"/> so it appears in VS Test Explorer's Test Detail Summary.
/// </summary>
public class TestBase
{
    public TestBase(ITestOutputHelper output)
    {
        Console.SetOut(new OutputHelperWriter(output));
    }
}

/// <summary>
/// Bridges <see cref="Console"/> output to xUnit's <see cref="ITestOutputHelper"/>.
/// </summary>
class OutputHelperWriter(ITestOutputHelper output) : System.IO.StringWriter
{
    public override void WriteLine(string? value) => output.WriteLine(value ?? "");
}
