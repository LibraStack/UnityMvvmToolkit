using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityMvvmToolkit.Core.Internal.Interfaces
{
    internal interface IClassMemberProvider
    {
        void GetBindingContextMembers(Type bindingContextType, IDictionary<int, MemberInfo> result);
    }
}