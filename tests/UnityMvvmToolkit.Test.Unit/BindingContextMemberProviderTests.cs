using System.Reflection;
using FluentAssertions;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal;
using UnityMvvmToolkit.Core.Internal.Helpers;
using UnityMvvmToolkit.Test.Unit.TestBindingContext;

namespace UnityMvvmToolkit.Test.Unit;

public class BindingContextMemberProviderTests
{
    private readonly BindingContextMemberProvider _memberProvider;

    public BindingContextMemberProviderTests()
    {
        _memberProvider = new BindingContextMemberProvider();
    }

    [Theory]
    [MemberData(nameof(TestBindingContextData))]
    public void GetBindingContextMembers_ShouldReturnBindingContextMembers_WhenContextIsValid(Type bindingContextType,
        (int, MemberTypes)[] membersData)
    {
        // Arrange
        var members = new Dictionary<int, MemberInfo>();

        // Act
        _memberProvider.GetBindingContextMembers(bindingContextType, members);

        // Assert
        members.Count.Should().Be(membersData.Length);

        // Assert
        foreach (var (memberHash, memberType) in membersData)
        {
            members.Should().ContainKey(memberHash);
            members[memberHash].MemberType.Should().Be(memberType);
        }
    }

    [Fact]
    public void GetBindingContextMembers_ShouldReturnContextMembers_WhenDifferentContextsShareFieldsWithSameName()
    {
        // Arrange
        var members = new Dictionary<int, MemberInfo>();
        var bindingContextType1 = typeof(SameFieldNameBindingContext1);
        var bindingContextType2 = typeof(SameFieldNameBindingContext2);

        var nameMemberHash1 = HashCodeHelper.GetMemberHashCode(bindingContextType1, "Name");
        var nameMemberHash2 = HashCodeHelper.GetMemberHashCode(bindingContextType1, "Name");

        // Act
        _memberProvider.GetBindingContextMembers(bindingContextType1, members);
        _memberProvider.GetBindingContextMembers(bindingContextType2, members);

        // Assert
        members.Count.Should().Be(2);

        members.Should().ContainKey(nameMemberHash1);
        members.Should().ContainKey(nameMemberHash2);

        members[nameMemberHash1].MemberType.Should().Be(MemberTypes.Field);
        members[nameMemberHash2].MemberType.Should().Be(MemberTypes.Field);
    }


    [Fact]
    public void GetBindingContextMembers_ShouldThrow_WhenFieldNamesAreInvalid()
    {
        // Arrange
        var members = new Dictionary<int, MemberInfo>();
        var bindingContextType = typeof(InvalidFieldNameBindingContext);

        // Assert
        _memberProvider
            .Invoking(sut => sut.GetBindingContextMembers(bindingContextType, members))
            .Should()
            .Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetBindingContextMembers_ShouldThrow_WhenAttributesShareSameName()
    {
        // Arrange
        var members = new Dictionary<int, MemberInfo>();
        var bindingContextType = typeof(ObservableFieldWithSameNameBindingContext);

        // Assert
        _memberProvider
            .Invoking(sut => sut.GetBindingContextMembers(bindingContextType, members))
            .Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void GetBindingContextMembers_ShouldThrow_WhenTypeIsNotAssignableFromIBindingContext()
    {
        // Arrange
        var bindingContextType = typeof(NoBindingContext);

        // Assert
        _memberProvider
            .Invoking(sut => sut.GetBindingContextMembers(bindingContextType, null))
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"{nameof(NoBindingContext)} is not assignable from {nameof(IBindingContext)}.");
    }

    [Fact]
    public void GetBindingContextMembers_ShouldThrow_WhenResultsDictionaryIsNull()
    {
        // Arrange
        var bindingContextType = typeof(PrivateFieldBindingContext);

        // Assert
        _memberProvider
            .Invoking(sut => sut.GetBindingContextMembers(bindingContextType, null))
            .Should()
            .Throw<NullReferenceException>();
    }

    private static IEnumerable<object[]> TestBindingContextData()
    {
        yield return GetPrivateFieldBindingContextTestData();
        yield return GetObservableFieldBindingContextTestData();
        yield return GetPublicFieldBindingContextTestData();
        yield return GetPublicPropertyBindingContextTestData();
        yield return GetCommandBindingContextTestData();
        yield return GetNoObservableFieldsBindingContextTestData();
    }

    private static object[] GetPrivateFieldBindingContextTestData()
    {
        var bindingContextType = typeof(PrivateFieldBindingContext);
        return new object[]
        {
            bindingContextType,
            new (int, MemberTypes)[]
            {
                (HashCodeHelper.GetMemberHashCode(bindingContextType, "Bool"), MemberTypes.Field),
                (HashCodeHelper.GetMemberHashCode(bindingContextType, "Int"), MemberTypes.Field),
                (HashCodeHelper.GetMemberHashCode(bindingContextType, "Float"), MemberTypes.Field),
                (HashCodeHelper.GetMemberHashCode(bindingContextType, "Str"), MemberTypes.Field)
            }
        };
    }

    private static object[] GetObservableFieldBindingContextTestData()
    {
        var bindingContextType = typeof(ObservableFieldBindingContext);
        return new object[]
        {
            bindingContextType,
            new (int, MemberTypes)[]
            {
                (HashCodeHelper.GetMemberHashCode(bindingContextType, "Bool"), MemberTypes.Field),
                (HashCodeHelper.GetMemberHashCode(bindingContextType, "BoolName"), MemberTypes.Field)
            }
        };
    }

    private static object[] GetPublicFieldBindingContextTestData()
    {
        var bindingContextType = typeof(PublicFieldBindingContext);
        return new object[]
        {
            bindingContextType,
            new (int, MemberTypes)[]
            {
                (HashCodeHelper.GetMemberHashCode(bindingContextType, nameof(PublicFieldBindingContext.Bool)),
                    MemberTypes.Field),
                (HashCodeHelper.GetMemberHashCode(bindingContextType, nameof(PublicFieldBindingContext.Int)),
                    MemberTypes.Field),
                (HashCodeHelper.GetMemberHashCode(bindingContextType, nameof(PublicFieldBindingContext.Float)),
                    MemberTypes.Field),
                (HashCodeHelper.GetMemberHashCode(bindingContextType, nameof(PublicFieldBindingContext.Str)),
                    MemberTypes.Field)
            }
        };
    }

    private static object[] GetPublicPropertyBindingContextTestData()
    {
        var bindingContextType = typeof(PublicPropertyBindingContext);
        return new object[]
        {
            bindingContextType,
            new (int, MemberTypes)[]
            {
                (HashCodeHelper.GetMemberHashCode(bindingContextType, nameof(PublicPropertyBindingContext.Bool)),
                    MemberTypes.Property),
                (HashCodeHelper.GetMemberHashCode(bindingContextType, nameof(PublicPropertyBindingContext.Int)),
                    MemberTypes.Property),
                (HashCodeHelper.GetMemberHashCode(bindingContextType, nameof(PublicPropertyBindingContext.Float)),
                    MemberTypes.Property),
                (HashCodeHelper.GetMemberHashCode(bindingContextType, nameof(PublicPropertyBindingContext.Str)),
                    MemberTypes.Property)
            }
        };
    }

    private static object[] GetCommandBindingContextTestData()
    {
        var bindingContextType = typeof(CommandBindingContext);
        return new object[]
        {
            bindingContextType,
            new (int, MemberTypes)[]
            {
                (HashCodeHelper.GetMemberHashCode(bindingContextType, nameof(CommandBindingContext.IncrementCommand)),
                    MemberTypes.Property),
                (HashCodeHelper.GetMemberHashCode(bindingContextType, nameof(CommandBindingContext.DecrementCommand)),
                    MemberTypes.Property),
                (HashCodeHelper.GetMemberHashCode(bindingContextType, nameof(CommandBindingContext.SetValueCommand)),
                    MemberTypes.Property)
            }
        };
    }

    private static object[] GetNoObservableFieldsBindingContextTestData()
    {
        return new object[] { typeof(NoObservableFieldsBindingContext), Array.Empty<(int, MemberTypes)>() };
    }
}