using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.Helpers
{
    internal static class ObjectWrapperHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IPropertyWrapper CreatePropertyWrapper(Type type, object[] args, int converterId,
            IBaseProperty property)
        {
            var propertyWrapper = (IPropertyWrapper) Activator.CreateInstance(type, args);

            return property is null
                ? propertyWrapper.SetConverterId(converterId)
                : propertyWrapper.SetConverterId(converterId).SetProperty(property);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICommandWrapper CreateCommandWrapper(Type type, object[] args, int converterId, int commandId,
            IBaseCommand command)
        {
            var commandWrapper = (ICommandWrapper) Activator.CreateInstance(type, args);

            return command is null
                ? commandWrapper.SetConverterId(converterId)
                : commandWrapper.SetConverterId(converterId).SetCommand(commandId, command);
        }
    }
}