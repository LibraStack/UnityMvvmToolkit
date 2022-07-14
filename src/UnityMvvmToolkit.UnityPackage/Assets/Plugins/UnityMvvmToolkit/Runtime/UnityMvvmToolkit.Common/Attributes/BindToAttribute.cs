using System;

namespace UnityMvvmToolkit.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class BindToAttribute : Attribute
    {
        public BindToAttribute(string targetPropertyName) // TODO: One or two way (set from uxml).
        {
            TargetPropertyName = targetPropertyName;
        }

        public string TargetPropertyName { get; }
    }
}