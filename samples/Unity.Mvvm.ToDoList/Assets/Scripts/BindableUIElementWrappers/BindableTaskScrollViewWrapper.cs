using Controllers;
using Extensions;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK.BindableUIElements;
using UnityMvvmToolkit.UITK.BindableUIElementWrappers;

namespace BindableUIElementWrappers
{
    public class BindableTaskScrollViewWrapper : BindableScrollViewWrapper<TaskItemController, TaskItemData>
    {
        public BindableTaskScrollViewWrapper(BindableScrollView listView, VisualTreeAsset itemAsset,
            IObjectProvider objectProvider) : base(listView, itemAsset, objectProvider)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            ScrollView.contentViewport.style.overflow = Overflow.Visible;
            ScrollView.contentContainer.style.overflow = Overflow.Visible;
        }

        protected override TaskItemController OnMakeItem(VisualElement itemAsset)
        {
            return new TaskItemController(itemAsset, OnTaskStateChanged, OnTaskRemoveClicked);
        }

        protected override void OnBindItem(TaskItemController item, TaskItemData data)
        {
            item.SetData(data);
        }

        private void OnTaskStateChanged(TaskItemData data)
        {
            ItemsCollection.Update(data);
        }

        private void OnTaskRemoveClicked(TaskItemData data)
        {
            ItemsCollection.Remove(data);
        }
    }
}