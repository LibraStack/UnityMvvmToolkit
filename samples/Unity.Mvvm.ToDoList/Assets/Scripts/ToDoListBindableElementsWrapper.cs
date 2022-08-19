using BindableUIElements;
using BindableUIElementWrappers;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UI;
using UnityMvvmToolkit.UI.BindableUIElements;

public class ToDoListBindableElementsWrapper : BindableElementsWrapper
{
    private readonly VisualTreeAsset _taskItemAsset;

    public ToDoListBindableElementsWrapper(VisualTreeAsset taskItemAsset)
    {
        _taskItemAsset = taskItemAsset;
    }

    public override IBindableElement Wrap(IBindableUIElement bindableUiElement, IObjectProvider objectProvider)
    {
        return bindableUiElement switch
        {
            BindableListView listView => new BindableTaskListWrapper(listView, _taskItemAsset, objectProvider),
            BindableScrollView scrollView => new BindableTaskScrollViewWrapper(scrollView, _taskItemAsset, objectProvider),
            BindableAddTaskButton addTaskButton => new BindableAddTaskButtonWrapper(addTaskButton, objectProvider),

            _ => base.Wrap(bindableUiElement, objectProvider)
        };
    }
}