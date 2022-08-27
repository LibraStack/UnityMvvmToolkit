using System;
using System.Runtime.CompilerServices;
using BindableUIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Controllers
{
    public class MobileInputDialogController : IDisposable
    {
        private readonly Button _applyButton;
        private readonly TextField _textField;

        public MobileInputDialogController(VisualElement mobileDialog)
        {
            _textField = mobileDialog.Q<TextField>();
            _textField.RegisterCallback<FocusInEvent>(OnTextFieldFocusIn);

            _applyButton = mobileDialog.Q<Button>();
            _applyButton.clicked += OnApplyButtonClick;
        }

        public bool IsMobileInputHidden()
        {
            if (_textField is BindableMobileInputField mobileInputField)
            {
                return mobileInputField.HideMobileInput;
            }

            return false;
        }

        public void HideScreenKeyboard()
        {
            TryHideScreenKeyboard();
        }

        public void Dispose()
        {
            _textField.UnregisterCallback<FocusInEvent>(OnTextFieldFocusIn);
            _applyButton.clicked -= OnApplyButtonClick;
        }

        private void OnTextFieldFocusIn(FocusInEvent e)
        {
            TouchScreenKeyboard.Android.consumesOutsideTouches = false;
        }

        private void OnApplyButtonClick()
        {
            TryHideScreenKeyboard();
            TouchScreenKeyboard.Android.consumesOutsideTouches = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TryHideScreenKeyboard()
        {
            if (TouchScreenKeyboard.visible == false)
            {
                return;
            }

            var keyboardInstance = TouchScreenKeyboard.Open(string.Empty);
            if (keyboardInstance is { active: true })
            {
                keyboardInstance.active = false;
            }
        }
    }
}