using FluentAssertions;
using UnityMvvmToolkit.Core.Internal.Structs;

namespace UnityMvvmToolkit.Test.Unit;

public class LineSplitEnumeratorTests
{
    [Theory]
    [InlineData("1,2,3,4,5", ',', 5)]
    [InlineData(" 1 , 2 , 3 , 4 , 5 ", ',', 5)]
    public void Split_ShouldSplitString_WhenStringIsValid(string str, char separator, int expectation)
    {
        // Arrange
        var index = 0;
        var lineEnumerator = new LineSplitEnumerator(str, separator, true);

        // Act
        foreach (var line in lineEnumerator)
        {
            line.Index.Should().Be(index);
            index++;
        }

        // Assert
        index.Should().Be(expectation);
    }
}