using System;
using System.Reflection;
using UnityMvvmToolkit.Core.Enums;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IObjectProvider
    {
        IObjectProvider WarmupAssemblyViewModels();
        IObjectProvider WarmupAssemblyViewModels(Assembly assembly);
        IObjectProvider WarmupViewModel<TBindingContext>() where TBindingContext : IBindingContext;
        IObjectProvider WarmupViewModel(Type bindingContextType);

        IObjectProvider WarmupValueConverter<T>(int capacity, WarmupType warmupType = WarmupType.OnlyByType)
            where T : IValueConverter;

        IProperty<TValueType> RentProperty<TValueType>(IBindingContext context, PropertyBindingData bindingData);
        void ReturnProperty<TValueType>(IProperty<TValueType> property);

        IReadOnlyProperty<TValueType> RentReadOnlyProperty<TValueType>(IBindingContext context,
            PropertyBindingData bindingData);

        void ReturnReadOnlyProperty<TValueType>(IReadOnlyProperty<TValueType> property);

        TCommand GetCommand<TCommand>(IBindingContext context, string propertyName) where TCommand : IBaseCommand;

        IBaseCommand RentCommandWrapper(IBindingContext context, CommandBindingData bindingData);
        void ReturnCommandWrapper(IBaseCommand command, CommandBindingData bindingData);

        TValue GetCollectionItemTemplate<TKey, TValue>();
    }
}