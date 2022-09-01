using BindableUIElements;
using BindableUIElementWrappers;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK;
using UnityMvvmToolkit.UITK.BindableUIElements;

public class ToDoListBindableElementsFactory : BindableElementsFactory
{
    private readonly VisualTreeAsset _taskItemAsset;

    public ToDoListBindableElementsFactory(VisualTreeAsset taskItemAsset)
    {
        _taskItemAsset = taskItemAsset;
    }

    public override IBindableElement Create(IBindableUIElement bindableUiElement, IObjectProvider objectProvider)
    {
        return bindableUiElement switch
        {
            BindableScrollView scrollView => new BindableTaskScrollViewWrapper(scrollView, _taskItemAsset, objectProvider),
            BindablePageBlocker pageBlocker => new BindableBinaryStateButtonWrapper(pageBlocker, objectProvider),
            BindableAddTaskButton addTaskButton => new BindableBinaryStateButtonWrapper(addTaskButton, objectProvider),

            _ => base.Create(bindableUiElement, objectProvider)
        };
    }
}