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
