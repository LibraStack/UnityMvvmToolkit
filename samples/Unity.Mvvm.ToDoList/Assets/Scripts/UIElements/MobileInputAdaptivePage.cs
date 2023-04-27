using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Controllers;
using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine;
using UnityEngine.UIElements;
using UnityMvvmToolkit.UniTask;
using Utilities;

namespace UIElements
{
    public class MobileInputAdaptivePage : VisualElement, IKeyboardHeightRecipient, IDisposable
    {
        private bool _isActivated;
        private float _parentHeight;
        private float _initialPaddingBottom;
        private float _defaultPaddingBottom;
        private float? _paddingBottomWithKeyboard;

        private MobileInputDialogController _inputDialog;
        private CancellationTokenSource _cancellationTokenSource;

        public MobileInputAdaptivePage()
        {
            if (Application.isPlaying)
            {
                RegisterCallback<GeometryChangedEvent>(OnLayoutCalculated);
            }
        }

        public float OffsetFromKeyboardPx { get; set; }

        public async UniTask ActivateAsync()
        {
            if (_isActivated)
            {
                return;
            }
            
            _isActivated = true;
            
            if (IsScreenKeyboardSupported())
            {
                TrackKeyboardActivityAsync().Forget();
            }

            SetVisible(true);
            SetOpacity(1);
            SetPaddingBottom(_defaultPaddingBottom);

            await this.WaitForLongestTransitionEnd();
        }

        public async UniTask DeactivateAsync()
        {
            if (_isActivated == false)
            {
                return;
            }
            
            _isActivated = false;
            
            if (IsScreenKeyboardSupported())
            {
                _inputDialog.HideScreenKeyboard();
                _cancellationTokenSource?.Cancel();
            }

            SetOpacity(0);
            SetPaddingBottom(_initialPaddingBottom);

            await this.WaitForLongestTransitionEnd();

            SetVisible(false);
        }

        public void Dispose()
        {
            _inputDialog?.Dispose();
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        private void OnLayoutCalculated(GeometryChangedEvent evt)
        {
            _parentHeight = parent.resolvedStyle.height;

            _initialPaddingBottom = 0;
            _defaultPaddingBottom = resolvedStyle.paddingBottom;
            
            _inputDialog = new MobileInputDialogController(this);

            SetVisible(false);
            SetOpacity(0);
            SetPaddingBottom(_initialPaddingBottom);
            
            UnregisterCallback<GeometryChangedEvent>(OnLayoutCalculated);
        }

        private void SetVisible(bool value)
        {
            parent.visible = value;
        }

        private async UniTaskVoid TrackKeyboardActivityAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var cancellationToken = _cancellationTokenSource.Token;

                while (_isActivated)
                {
                    if (TouchScreenKeyboard.visible == false)
                    {
                        SetPaddingBottom(_defaultPaddingBottom);
                        await UniTask.Yield(cancellationToken);

                        continue;
                    }

                    if (_paddingBottomWithKeyboard.HasValue)
                    {
                        SetPaddingBottom(_paddingBottomWithKeyboard.Value);
                        await UniTask.Yield(cancellationToken);

                        continue;
                    }

                    await ResizePageAsync(cancellationToken);
                }
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsScreenKeyboardSupported()
        {
            return TouchScreenKeyboard.isSupported;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetOpacity(float value)
        {
            if (Math.Abs(resolvedStyle.opacity - value) > float.Epsilon)
            {
                style.opacity = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetPaddingBottom(float value)
        {
            if (Math.Abs(resolvedStyle.paddingBottom - value) > float.Epsilon)
            {
                style.paddingBottom = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async UniTask ResizePageAsync(CancellationToken cancellationToken)
        {
            var includeInput = _inputDialog.IsMobileInputHidden() == false;
            var keyboardHeight =
                await MobileUtilities.GetRelativeKeyboardHeightAsync(includeInput, _parentHeight, this,
                    cancellationToken);

            _paddingBottomWithKeyboard = keyboardHeight + OffsetFromKeyboardPx;
        }

        public void ReceiveHeight(int keyboardHeight)
        {
            style.paddingBottom = keyboardHeight > _defaultPaddingBottom
                ? keyboardHeight + OffsetFromKeyboardPx
                : _defaultPaddingBottom;
        }

        public new class UxmlFactory : UxmlFactory<MobileInputAdaptivePage, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlFloatAttributeDescription _offsetFromKeyboardAttribute = new()
                { name = "offset-from-keyboard-px", defaultValue = default };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                ((MobileInputAdaptivePage) visualElement).OffsetFromKeyboardPx =
                    _offsetFromKeyboardAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}