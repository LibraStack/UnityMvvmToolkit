using System;

namespace UnityMvvmToolkit.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class VisualTreeAssetAttribute : Attribute
    {
        public VisualTreeAssetAttribute(string assetPath)
        {
            AssetPath = assetPath;
        }

        public string AssetPath { get; }
    }
}