using UIElements;
using UnityEngine.UIElements;

namespace BindableUIElements
{
    public partial class BindableContentPage
    {
        public string BindingThemeModePath { get; private set; }

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