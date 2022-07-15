using UnityMvvmToolkit.Common;

namespace ViewModels
{
    public class MainMenuViewModel : ViewModel
    {
        private string _strValue;

        public string StrValue
        {
            get => _strValue;
            set => Set(ref _strValue, value);
        }
    }
}