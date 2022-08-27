using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine;

namespace Utilities
{
    public static class MobileUtilities
    {
        private const int UndefinedValue = -1;
        private static int ScreenHeight => Screen.height;

        private static int _keyboardHeight = UndefinedValue;

        public static int GetRelativeKeyboardHeight(float uiHeight, bool includeInput = true)
        {
            var keyboardHeight = GetKeyboardHeight(includeInput);
            var screenToRectRatio = ScreenHeight / uiHeight;

            return (int) (keyboardHeight / screenToRectRatio);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetKeyboardHeight(bool includeInput)
        {
            if (Application.isEditor)
            {
                return _keyboardHeight = 0;
            }

            GetIosKeyboardHeight(keyboardHeight => _keyboardHeight = keyboardHeight);
            GetAndroidKeyboardHeight(keyboardHeight => _keyboardHeight = keyboardHeight, includeInput);

            return _keyboardHeight;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<int> GetKeyboardHeightAsync(bool includeInput,
            IKeyboardHeightRecipient heightRecipient = null, CancellationToken cancellationToken = default)
        {
            return GetKeyboardHeightAsync(includeInput, null, heightRecipient, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<int> GetRelativeKeyboardHeightAsync(bool includeInput, float contentPageHeight,
            IKeyboardHeightRecipient heightRecipient = null, CancellationToken cancellationToken = default)
        {
            return GetKeyboardHeightAsync(includeInput, contentPageHeight, heightRecipient, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async UniTask<int> GetKeyboardHeightAsync(bool includeInput, float? contentPageHeight,
            IKeyboardHeightRecipient heightRecipient, CancellationToken cancellationToken)
        {
            var result = 0;
            var iterations = 100;
            var keyboardHeight = int.MinValue;
            var screenToRectRatio = ScreenHeight / contentPageHeight;

            while (result > keyboardHeight)
            {
                keyboardHeight = screenToRectRatio.HasValue
                    ? (int) (GetKeyboardHeight(includeInput) / screenToRectRatio.Value)
                    : GetKeyboardHeight(includeInput);

                if (keyboardHeight > result)
                {
                    result = keyboardHeight;
                    keyboardHeight = int.MinValue;
                    heightRecipient?.ReceiveHeight(result);
                }
                else if (keyboardHeight == result)
                {
                    iterations--;
                    keyboardHeight = int.MinValue;
                }

                if (iterations == 0)
                {
                    break;
                }

                await UniTask.Yield(cancellationToken);
            }

            return result;
        }

        [Conditional("UNITY_IOS")]
        private static void GetIosKeyboardHeight(Action<int> callback)
        {
            var keyboardHeight = Mathf.RoundToInt(TouchScreenKeyboard.area.height);
            callback(keyboardHeight >= ScreenHeight ? int.MinValue : keyboardHeight);
        }

        [Conditional("UNITY_ANDROID")]
        private static void GetAndroidKeyboardHeight(Action<int> callback, bool includeInput)
        {
            using var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

            var unityPlayer = unityClass
                .GetStatic<AndroidJavaObject>("currentActivity")
                .Get<AndroidJavaObject>("mUnityPlayer");

            var view = unityPlayer.Call<AndroidJavaObject>("getView");
            var dialog = unityPlayer.Get<AndroidJavaObject>("mSoftInputDialog");

            if (view == null || dialog == null)
            {
                callback(UndefinedValue);
                return;
            }

            var decorHeight = 0;

            if (includeInput)
            {
                var decorView = dialog.Call<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView");
                if (decorView != null)
                {
                    decorHeight = decorView.Call<int>("getHeight");
                }
            }

            using var rect = new AndroidJavaObject("android.graphics.Rect");

            view.Call("getWindowVisibleDisplayFrame", rect);

            callback(ScreenHeight - rect.Call<int>("height") + decorHeight);
        }
    }
}