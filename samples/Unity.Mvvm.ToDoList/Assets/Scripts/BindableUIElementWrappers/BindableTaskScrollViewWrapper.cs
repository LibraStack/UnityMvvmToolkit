using Controllers;
using Extensions;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK.BindableUIElements;
// using UnityMvvmToolkit.UITK.BindableUIElementWrappers;
using ViewModels;
using Views;

namespace BindableUIElementWrappers
{
    public class BindableTaskScrollViewWrapper //: BindableScrollViewWrapper<TaskItemView, TaskItemViewModel>
    {
        // public BindableTaskScrollViewWrapper(BindableScrollView listView, VisualTreeAsset itemAsset,
        //     IObjectProvider objectProvider) : base(listView, itemAsset, objectProvider)
        // {
        // }
        //
        // public override void Initialize()
        // {
        //     base.Initialize();
        //
        //     ScrollView.contentViewport.style.overflow = Overflow.Visible;
        //     ScrollView.contentContainer.style.overflow = Overflow.Visible;
        // }
        //
        // protected override TaskItemView OnMakeItem(VisualElement itemAsset)
        // {
        //     return new TaskItemView(itemAsset, OnTaskStateChanged, OnTaskRemoveClicked);
        // }
        //
        // protected override void OnBindItem(TaskItemView item, TaskItemViewModel viewModel)
        // {
        //     item.SetData(viewModel);
        // }
        //
        // private void OnTaskStateChanged(TaskItemViewModel viewModel)
        // {
        //     ItemsCollection.Update(viewModel);
        // }
        //
        // private void OnTaskRemoveClicked(TaskItemViewModel viewModel)
        // {
        //     ItemsCollection.Remove(viewModel);
        // }
    }
}