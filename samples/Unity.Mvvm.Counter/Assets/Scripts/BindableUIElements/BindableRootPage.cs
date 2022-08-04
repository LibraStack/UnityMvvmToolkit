using UIElements;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;

namespace BindableUIElements
{
    public class BindableRootPage : RootPage, IBindableUIElement
    {
        public string BindingThemeModePath { get; set; }

        public new class UxmlFactory : UxmlFactory<BindableRootPage, UxmlTraits>
        {
        }

        public new class UxmlTraits : RootPage.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingThemeModeAttribute = new()
                { name = "binding-theme-mode-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                ((BindableRootPage) visualElement).BindingThemeModePath =
                    _bindingThemeModeAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}