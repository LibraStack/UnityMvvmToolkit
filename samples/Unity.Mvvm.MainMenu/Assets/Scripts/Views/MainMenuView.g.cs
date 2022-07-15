// using System;
// using System.Runtime.CompilerServices;
// using UnityEngine.UIElements;
// using UnityMvvmToolkit.Common.Interfaces;
// using UnityMvvmToolkit.UI.BindableVisualElements;

namespace Views
{
    // public partial class MainMenuView
    // {
    //     protected override IVisualElementBindings GetVisualElementsBindings(ViewModels.MainMenuViewModel bindingContext,
    //         IBindableVisualElement bindableElement)
    //     {
    //         return bindableElement switch
    //         {
    //             BindableLabel bindableLabel => new LabelBindings(bindingContext, bindableLabel),
    //             BindableTextField bindableTextField => new TextFieldBindings(bindingContext, bindableTextField),
    //             _ => default
    //         };
    //     }
    //
    //     private class LabelBindings : IVisualElementBindings
    //     {
    //         private readonly ViewModels.MainMenuViewModel _viewModel;
    //         private readonly BindableLabel _visualElement;
    //
    //         public LabelBindings(ViewModels.MainMenuViewModel viewModel, BindableLabel visualElement)
    //         {
    //             _viewModel = viewModel;
    //             _visualElement = visualElement;
    //         }
    //
    //         public void UpdateValues()
    //         {
    //             UpdateStrValue();
    //         }
    //
    //         [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //         private void UpdateStrValue()
    //         {
    //             if (_visualElement.text != _viewModel.StrValue)
    //             {
    //                 _visualElement.text = _viewModel.StrValue;
    //             }
    //         }
    //     }
    //
    //     private class TextFieldBindings : IVisualElementBindings, IDisposable
    //     {
    //         private readonly ViewModels.MainMenuViewModel _viewModel;
    //         private readonly BindableTextField _visualElement;
    //
    //         public TextFieldBindings(ViewModels.MainMenuViewModel viewModel, BindableTextField visualElement)
    //         {
    //             _viewModel = viewModel;
    //
    //             _visualElement = visualElement;
    //             _visualElement.RegisterValueChangedCallback(OnVisualElementValueChanged);
    //         }
    //
    //         public void Dispose()
    //         {
    //             _visualElement.UnregisterValueChangedCallback(OnVisualElementValueChanged);
    //         }
    //
    //         public void UpdateValues()
    //         {
    //             UpdateStrValue();
    //         }
    //
    //         private void OnVisualElementValueChanged(ChangeEvent<string> e)
    //         {
    //             _viewModel.StrValue = e.newValue;
    //         }
    //
    //         [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //         private void UpdateStrValue()
    //         {
    //             if (_visualElement.value != _viewModel.StrValue)
    //             {
    //                 _visualElement.SetValueWithoutNotify(_viewModel.StrValue);
    //             }
    //         }
    //     }
    // }
}