using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public partial class BindableListView : BaseBindableListView<ICollectionItem>
    {
        private PropertyBindingData _itemsSourceBindingData;
        private PropertyBindingData _itemTemplateBindingData;

        protected override PropertyBindingData GetItemsSourceBindingData()
        {
            return _itemsSourceBindingData ??= BindingItemsSourcePath.ToPropertyBindingData();
        }

        protected override PropertyBindingData GetItemTemplateBindingData()
        {
            return _itemTemplateBindingData ??= BindingItemTemplatePath.ToPropertyBindingData();
        }
    }
}