using Beater;

namespace Tests;

public class SerializationTests
{
    [Fact]
    public void FromJsonTest()
    {
        // arrange
        var filePath = "json/test-sequence.json";

        // act
        var json = File.ReadAllText(filePath);
        var sequence = SequenceDesign.FromJson(json);

        // assert
        sequence.Should().NotBeNull();
    }
}
