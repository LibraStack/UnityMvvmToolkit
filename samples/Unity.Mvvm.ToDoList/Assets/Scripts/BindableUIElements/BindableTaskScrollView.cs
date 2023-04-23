using UnityEngine.UIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.UITK.BindableUIElements;
using ViewModels;

namespace BindableUIElements
{
    public partial class BindableTaskScrollView : BaseBindableScrollView<TaskItemViewModel>
    {
        private PropertyBindingData _itemsSourceBindingData;
        private PropertyBindingData _itemTemplateBindingData;

        public override void Initialize()
        {
            base.Initialize();

            contentViewport.style.overflow = Overflow.Visible;
            contentContainer.style.overflow = Overflow.Visible;
        }

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