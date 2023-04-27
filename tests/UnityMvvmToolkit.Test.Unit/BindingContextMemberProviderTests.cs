﻿using System.Reflection;
using FluentAssertions;
using UnityMvvmToolkit.Core.Internal;
using UnityMvvmToolkit.Core.Internal.Helpers;
using UnityMvvmToolkit.Test.Unit.TestBindingContext;

namespace UnityMvvmToolkit.Test.Unit;

public class BindingContextMemberProviderTests
{
    [Fact]
    public void GetBindingContextMembers_ShouldReturnBindingContextMembers()
    {
        // Arrange
        var membersProvider = new BindingContextMemberProvider();
        var members = new Dictionary<int, MemberInfo>();

        var bindingContextType = typeof(MyBindingContext);

        var titleMemberHash =
            HashCodeHelper.GetMemberHashCode(bindingContextType, nameof(MyBindingContext.Title));

        var countMemberHash =
            HashCodeHelper.GetMemberHashCode(bindingContextType, nameof(MyBindingContext.Count));

        var descriptionMemberHash =
            HashCodeHelper.GetMemberHashCode(bindingContextType, nameof(MyBindingContext.Description));

        var incrementMemberHash =
            HashCodeHelper.GetMemberHashCode(bindingContextType, nameof(MyBindingContext.IncrementCommand));

        var decrementMemberHash =
            HashCodeHelper.GetMemberHashCode(bindingContextType, nameof(MyBindingContext.DecrementCommand));

        var setValueMemberHash =
            HashCodeHelper.GetMemberHashCode(bindingContextType, nameof(MyBindingContext.SetValueCommand));

        // Act
        membersProvider.GetBindingContextMembers(bindingContextType, members);

        // Assert
        members.Count.Should().Be(6);

        members.Should().ContainKey(titleMemberHash);
        members.Should().ContainKey(countMemberHash);
        members.Should().ContainKey(descriptionMemberHash);
        members.Should().ContainKey(incrementMemberHash);
        members.Should().ContainKey(decrementMemberHash);
        members.Should().ContainKey(setValueMemberHash);

        members[titleMemberHash].MemberType.Should().Be(MemberTypes.Property);
        members[countMemberHash].MemberType.Should().Be(MemberTypes.Field);
        members[descriptionMemberHash].MemberType.Should().Be(MemberTypes.Field);
        members[incrementMemberHash].MemberType.Should().Be(MemberTypes.Property);
        members[decrementMemberHash].MemberType.Should().Be(MemberTypes.Property);
        members[setValueMemberHash].MemberType.Should().Be(MemberTypes.Property);
    }
}