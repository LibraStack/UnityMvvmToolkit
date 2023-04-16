using System;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IObjectProvider
    {
        IObjectProvider WarmupAssemblyViewModels();
        IObjectProvider WarmupViewModel<TBindingContext>() where TBindingContext : IBindingContext;
        IObjectProvider WarmupViewModel(Type bindingContextType);

        IProperty<TValueType> RentProperty<TValueType>(IBindingContext context, PropertyBindingData bindingData);
        void ReturnProperty<TValueType>(IProperty<TValueType> property);

        IReadOnlyProperty<TValueType> RentReadOnlyProperty<TValueType>(IBindingContext context,
            PropertyBindingData bindingData);
        void ReturnReadOnlyProperty<TValueType>(IReadOnlyProperty<TValueType> property);

        TCommand GetCommand<TCommand>(IBindingContext context, string propertyName) where TCommand : IBaseCommand;

        // TODO: Rent?
        ICommandWrapper GetCommandWrapper(IBindingContext context, CommandBindingData bindingData);
    }
}