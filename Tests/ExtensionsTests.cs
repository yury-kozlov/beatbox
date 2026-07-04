using Beater;

namespace Tests;

public class ExtensionsTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public void MoveBefore_IndexToMoveBeforeTargetIndex_MovesItemForward()
    {
        // arrange
        var source = new List<int> { 0, 1, 2, 3, 4 };

        // act
        source.MoveBefore(targetIndex: 3, indexToMove: 0);

        // assert
        source.Should().BeEquivalentTo([ 1, 2, 0, 3, 4 ], options => options.WithStrictOrdering());
    }

    [Fact]
    public void MoveBefore_IndexToMoveAfterTargetIndex_MovesItemBackward()
    {
        // arrange
        var source = new List<int> { 0, 1, 2, 3, 4 };

        // act
        source.MoveBefore(targetIndex: 1, indexToMove: 4);

        // assert
        source.Should().BeEquivalentTo([ 0, 4, 1, 2, 3 ], options => options.WithStrictOrdering());
    }

    [Fact]
    public void MoveBefore_IndexToMoveEqualsTargetIndex_DoesNotChangeList()
    {
        // arrange
        var source = new List<int> { 0, 1, 2, 3, 4 };

        // act
        source.MoveBefore(targetIndex: 2, indexToMove: 2);

        // assert
        source.Should().BeEquivalentTo([ 0, 1, 2, 3, 4 ], options => options.WithStrictOrdering());
    }

    [Fact]
    public void MoveBefore_TargetIndexAdjacentAfterIndexToMove_MovesItemOnePositionForward()
    {
        // arrange
        var source = new List<int> { 0, 1, 2 };

        // act
        source.MoveBefore(targetIndex: 1, indexToMove: 0);

        // assert
        source.Should().BeEquivalentTo([ 0, 1, 2 ], options => options.WithStrictOrdering());
    }

    [Fact]
    public void MoveBefore_TargetIndexAtEndOfList_MovesItemToLastPosition()
    {
        // arrange
        var source = new List<int> { 0, 1, 2, 3 };

        // act
        source.MoveBefore(targetIndex: 4, indexToMove: 0);

        // assert
        source.Should().BeEquivalentTo([ 1, 2, 3, 0 ], options => options.WithStrictOrdering());
    }

    [Fact]
    public void MoveBefore_TargetIndexZero_MovesItemToFirstPosition()
    {
        // arrange
        var source = new List<int> { 0, 1, 2, 3 };

        // act
        source.MoveBefore(targetIndex: 0, indexToMove: 3);

        // assert
        source.Should().BeEquivalentTo([ 3, 0, 1, 2 ], options => options.WithStrictOrdering());
    }

    [Fact]
    public void MoveBefore_ReferenceTypeItems_PreservesItemIdentity()
    {
        // arrange
        var a = new object();
        var b = new object();
        var c = new object();
        var source = new List<object> { a, b, c };

        // act
        source.MoveBefore(targetIndex: 0, indexToMove: 2);

        // assert
        source.Should().BeEquivalentTo([ c, a, b ], options => options.WithStrictOrdering());
    }
}
