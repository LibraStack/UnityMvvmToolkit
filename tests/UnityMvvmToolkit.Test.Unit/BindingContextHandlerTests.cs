using System.Reflection;
using FluentAssertions;
using NSubstitute;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal;
using UnityMvvmToolkit.Core.Internal.Interfaces;
using UnityMvvmToolkit.Core.Internal.ObjectHandlers;
using UnityMvvmToolkit.Test.Unit.TestBindingContext;

namespace UnityMvvmToolkit.Test.Unit;

public class BindingContextHandlerTests
{
    [Fact]
    public void TryRegisterBindingContext_ShouldReturnTrue_WhenBindingContextIsNotRegistered()
    {
        // Arrange
        var bindingContext = Substitute.For<IBindingContext>();
        var memberProvider = Substitute.For<IClassMemberProvider>();

        var bindingContextHandler = new BindingContextHandler(memberProvider);

        // Act
        var isRegistered = bindingContextHandler.TryRegisterBindingContext(bindingContext.GetType());

        // Assert
        isRegistered.Should().Be(true);

        memberProvider
            .Received(1)
            .GetBindingContextMembers(Arg.Any<Type>(), Arg.Any<IDictionary<int, MemberInfo>>());
    }

    [Fact]
    public void TryRegisterBindingContext_ShouldReturnFalse_WhenBindingContextIsRegistered()
    {
        // Arrange
        var bindingContext = Substitute.For<IBindingContext>();
        var memberProvider = Substitute.For<IClassMemberProvider>();

        var bindingContextHandler = new BindingContextHandler(memberProvider);

        // Act
        bindingContextHandler.TryRegisterBindingContext(bindingContext.GetType());
        var isRegistered = bindingContextHandler.TryRegisterBindingContext(bindingContext.GetType());

        // Assert
        isRegistered.Should().Be(false);

        memberProvider
            .Received(1)
            .GetBindingContextMembers(Arg.Any<Type>(), Arg.Any<IDictionary<int, MemberInfo>>());
    }

    [Fact]
    public void TryGetContextMemberInfo_ShouldReturnValidData_WhenBindingContextIsRegistered()
    {
        // Arrange
        var bindingContext = new PublicFieldBindingContext();
        var memberProvider = new BindingContextMemberProvider();

        var bindingContextHandler = new BindingContextHandler(memberProvider);

        // Act
        bindingContextHandler.TryRegisterBindingContext(bindingContext.GetType());

        var isSuccess = bindingContextHandler.TryGetContextMemberInfo(bindingContext.GetType(),
            nameof(PublicFieldBindingContext.StrField), out var memberInfo);

        // Assert
        isSuccess.Should().Be(true);

        memberInfo.MemberType.Should().Be(MemberTypes.Field);
        memberInfo.Name.Should().Be(nameof(PublicFieldBindingContext.StrField));
    }

    [Fact]
    public void TryGetContextMemberInfo_ShouldReturnValidData_WhenBindingContextIsNotRegistered()
    {
        // Arrange
        var bindingContext = new PublicFieldBindingContext();
        var memberProvider = new BindingContextMemberProvider();

        var bindingContextHandler = new BindingContextHandler(memberProvider);

        // Act
        var isSuccess = bindingContextHandler.TryGetContextMemberInfo(bindingContext.GetType(),
            nameof(PublicFieldBindingContext.StrField), out var memberInfo);

        // Assert
        isSuccess.Should().Be(true);

        memberInfo.MemberType.Should().Be(MemberTypes.Field);
        memberInfo.Name.Should().Be(nameof(PublicFieldBindingContext.StrField));
    }

    [Fact]
    public void Dispose_ShouldClearCollections()
    {
        // Arrange
        var bindingContext = Substitute.For<IBindingContext>();
        var memberProvider = Substitute.For<IClassMemberProvider>();

        var bindingContextHandler = new BindingContextHandler(memberProvider);

        // Act
        bindingContextHandler.TryRegisterBindingContext(bindingContext.GetType());

        bindingContextHandler.Dispose();

        var isRegistered = bindingContextHandler.TryRegisterBindingContext(bindingContext.GetType());

        // Assert
        isRegistered.Should().Be(true);

        memberProvider
            .Received(2)
            .GetBindingContextMembers(Arg.Any<Type>(), Arg.Any<IDictionary<int, MemberInfo>>());
    }
}