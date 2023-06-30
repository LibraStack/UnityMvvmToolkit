using System;
using System.Collections.Generic;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Common
{
    public sealed class BindableElementData : IDisposable
    {
        private readonly List<IBindableElement> _bindableElements;

        public BindableElementData(List<IBindableElement> bindableElements)
        {
            _bindableElements = bindableElements;
        }

        public IBindingContext BindingContext { get; set; }
        public IReadOnlyList<IBindableElement> BindableElements => _bindableElements;

        public void Dispose()
        {
            _bindableElements.Clear();
            BindingContext = default;
        }
    }
}