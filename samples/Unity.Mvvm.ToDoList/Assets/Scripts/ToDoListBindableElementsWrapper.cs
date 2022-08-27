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
            BindableScrollView scrollView => new BindableTaskScrollViewWrapper(scrollView, _taskItemAsset, objectProvider),
            BindablePageBlocker pageBlocker => new BindableBinaryStateButtonWrapper(pageBlocker, objectProvider),
            BindableAddTaskButton addTaskButton => new BindableBinaryStateButtonWrapper(addTaskButton, objectProvider),

            _ => base.Wrap(bindableUiElement, objectProvider)
        };
    }
}