using UnityMvvmToolkit.Common;

namespace ViewModels
{
    public class MainMenuViewModel : ViewModel
    {
        private string _strValue;
        private string _strValue1;

        public string StrValue
        {
            get => _strValue;
            set => Set(ref _strValue, value);
        }
        
        public string StrValue1
        {
            get => _strValue1;
            set => Set(ref _strValue1, value);
        }
    }
}