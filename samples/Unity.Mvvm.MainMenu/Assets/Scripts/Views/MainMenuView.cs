using System;
using System.ComponentModel;
using UnityMvvmToolkit.Common.Attributes;
using UnityMvvmToolkit.UI;
using ViewModels;

using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityMvvmToolkit.UI.BindableVisualElements;

namespace Views
{
    // [VisualTreeAsset("UI Toolkit/MainMenu.uxml")]
    public partial class MainMenuView : View<MainMenuViewModel>
    {
        protected override MainMenuViewModel GetBindingContext()
        {
            return new MainMenuViewModel { StrValue = "Test String!!!", StrValue1 = "It works!!!" };
        }
    }

    public partial class MainMenuView
    {
        private BindableLabel _label0;
        private BindableTextField _textField0;
        private BindableTextField _textField1;

        private Dictionary<string, Action> _updateValuesAction;

        protected override void BindElements(MainMenuViewModel bindingContext, VisualElement rootVisualElement)
        {
            _label0 = rootVisualElement.Q<BindableLabel>("label0");

            _textField0 = rootVisualElement.Q<BindableTextField>("textField0");
            _textField0.RegisterValueChangedCallback(OnUpdateStrValue);

            _textField1 = rootVisualElement.Q<BindableTextField>("textField1");
            _textField1.RegisterValueChangedCallback(OnUpdateStrValue1);

            _updateValuesAction = new Dictionary<string, Action>
            {
                { "StrValue", UpdateStrValueSubscribers },
                { "StrValue1", UpdateStrValue1Subscribers }
            };
            
            UpdateStrValueSubscribers();
            UpdateStrValue1Subscribers();
        }

        private void OnUpdateStrValue(ChangeEvent<string> e)
        {
            BindingContext.StrValue = e.newValue;
        }

        private void OnUpdateStrValue1(ChangeEvent<string> e)
        {
            BindingContext.StrValue1 = e.newValue;
        }

        private void UpdateStrValueSubscribers()
        {
            var newValue = BindingContext.StrValue;

            if (_label0.text != newValue)
            {
                _label0.text = newValue;
            }

            if (_textField0.value != newValue)
            {
                _textField0.SetValueWithoutNotify(newValue);
            }
        }

        private void UpdateStrValue1Subscribers()
        {
            var newValue = BindingContext.StrValue1;

            if (_textField1.value != newValue)
            {
                _textField1.SetValueWithoutNotify(newValue);
            }
        }

        protected override void OnBindingContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _updateValuesAction[e.PropertyName]();
        }
    }
}