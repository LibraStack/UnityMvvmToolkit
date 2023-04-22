using System;
using BindableUIElements;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UniTask.Interfaces;

namespace BindableUIElementWrappers
{
    public class BindableBinaryStateButtonWrapper //: BindableBinaryStateElementWrapper, IInitializable, IDisposable
    {
        private readonly BindableBinaryStateButton _binaryStateButton;
        private readonly IAsyncCommand _command;

        // public BindableBinaryStateButtonWrapper(BindableBinaryStateButton binaryStateButton,
        //     IObjectProvider objectProvider) : base(binaryStateButton, objectProvider)
        // {
        //     _binaryStateButton = binaryStateButton;
        //     _command = GetCommand<IAsyncCommand>(binaryStateButton.Command);
        // }
        //
        // public bool CanInitialize => _command != null;
        //
        // public void Initialize()
        // {
        //     _binaryStateButton.clicked += OnClicked;
        // }
        //
        // public void Dispose()
        // {
        //     _binaryStateButton.clicked -= OnClicked;
        // }
        //
        // private void OnClicked()
        // {
        //     _command.Execute();
        // }
    }
}