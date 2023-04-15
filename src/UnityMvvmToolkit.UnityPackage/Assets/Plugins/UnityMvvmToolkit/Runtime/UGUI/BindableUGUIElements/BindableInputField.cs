using TMPro;
using UnityEngine;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UGUI.BindableUGUIElements
{
    public class BindableInputField : MonoBehaviour//, IBindableUIElement
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private string _bindingTextPath;

        public TMP_InputField InputField => _inputField;
        public string BindingTextPath => _bindingTextPath;
    }
}