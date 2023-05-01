using FluentAssertions;
using UnityMvvmToolkit.Core.Internal.Helpers;

namespace UnityMvvmToolkit.Test.Unit;

public class HashCodeHelperTests
{
    [Fact]
    public void CombineHashCode_ShouldCombineHashCodes()
    {
        // Arrange
        var hash1 = typeof(bool).GetHashCode();
        var hash2 = typeof(int).GetHashCode();
        var hash3 = typeof(float).GetHashCode();

        var expected = HashCodeHelper.CombineHashCode(HashCodeHelper.CombineHashCode(hash1, hash2), hash3);

        // Act
        var result = HashCodeHelper.CombineHashCode(hash1, hash2, hash3);

        // Assert
        expected.Should().Be(result);
    }
}