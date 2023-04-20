using System;
using System.Collections.Generic;
using System.Reflection;
using UnityMvvmToolkit.Core.Internal.Helpers;
using UnityMvvmToolkit.Core.Internal.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.ObjectHandlers
{
    internal sealed class BindingContextHandler : IDisposable
    {
        private readonly HashSet<int> _initializedBindingContexts;
        private readonly Dictionary<int, MemberInfo> _memberInfos;

        private readonly IClassMemberProvider _classMemberProvider;

        public BindingContextHandler(IClassMemberProvider classMemberProvider)
        {
            _classMemberProvider = classMemberProvider;

            _initializedBindingContexts = new HashSet<int>();
            _memberInfos = new Dictionary<int, MemberInfo>();
        }

        public bool TryRegisterBindingContext(Type bindingContextType)
        {
            if (_initializedBindingContexts.Add(bindingContextType.GetHashCode()))
            {
                _classMemberProvider.GetBindingContextMembers(bindingContextType, _memberInfos);
                return true;
            }

            return false;
        }

        public bool TryGetContextMemberInfo(Type contextType, string memberName, out MemberInfo memberInfo)
        {
            TryRegisterBindingContext(contextType);

            return _memberInfos.TryGetValue(HashCodeHelper.GetMemberHashCode(contextType, memberName), out memberInfo);
        }

        public void Dispose()
        {
            _memberInfos.Clear();
            _initializedBindingContexts.Clear();
        }
    }
}