using Controllers;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UI.BindableUIElements;
using UnityMvvmToolkit.UI.BindableUIElementWrappers;

namespace BindableUIElementWrappers
{
    public class BindableTaskListWrapper : BindableListViewWrapper<TaskItemController, TaskItemData>
    {
        public BindableTaskListWrapper(BindableListView listView, VisualTreeAsset itemAsset,
            IObjectProvider objectProvider) : base(listView, itemAsset, objectProvider)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            var scrollView = ListView.Q<ScrollView>();
            scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            scrollView.contentViewport.style.overflow = Overflow.Visible;
            scrollView.contentContainer.style.overflow = Overflow.Visible;
            // scrollView.touchScrollBehavior = ScrollView.TouchScrollBehavior.Elastic;
            // scrollView.elasticity = 0.05f;
        }

        protected override TaskItemController OnMakeItem(VisualElement itemAsset)
        {
            return new TaskItemController(itemAsset);
        }

        protected override void OnBindItem(TaskItemController item, TaskItemData data)
        {
            item.SetData(data);
        }
    }
}