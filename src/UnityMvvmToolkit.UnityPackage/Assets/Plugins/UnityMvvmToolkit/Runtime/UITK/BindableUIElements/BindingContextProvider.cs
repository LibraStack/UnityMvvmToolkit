using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public partial class BindingContextProvider : BindingContextProvider<IBindingContext>
    {
        protected override IReadOnlyProperty<IBindingContext> RentReadOnlyProperty(IBindingContext context,
            IObjectProvider objectProvider, PropertyBindingData propertyBindingData)
        {
            return objectProvider.RentReadOnlyPropertyAs<IBindingContext>(context, propertyBindingData);
        }
    }
}