using UIElements;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;

namespace BindableUIElements
{
    public class BindableContentPage : ContentPage, IBindableUIElement
    {
        public string BindingThemeModePath { get; set; }

        public new class UxmlFactory : UxmlFactory<BindableContentPage, UxmlTraits>
        {
        }

        public new class UxmlTraits : ContentPage.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingThemeModeAttribute = new()
                { name = "binding-theme-mode-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                ((BindableContentPage) visualElement).BindingThemeModePath =
                    _bindingThemeModeAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}