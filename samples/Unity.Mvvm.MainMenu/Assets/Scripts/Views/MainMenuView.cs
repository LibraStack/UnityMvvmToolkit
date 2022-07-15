using UnityMvvmToolkit.Common.Attributes;
using UnityMvvmToolkit.UI;
using ViewModels;

namespace Views
{
    [VisualTreeAsset("UI Toolkit/MainMenu.uxml")]
    public partial class MainMenuView : View<MainMenuViewModel>
    {
        protected override MainMenuViewModel GetBindingContext()
        {
            return new MainMenuViewModel { StrValue = "Test String" };
        }
    }
}