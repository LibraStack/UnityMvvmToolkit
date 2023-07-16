namespace UnityMvvmToolkit.Core.Internal.ObjectWrappers
{
    internal sealed class PropertyCastWrapper<TSource, TValue> : PropertyWrapper<TSource, TValue>
    {
        public PropertyCastWrapper() : base(default)
        {
        }

        protected override TValue Convert(TSource value)
        {
            return (TValue) (object) value;
        }

        protected override TSource ConvertBack(TValue value)
        {
            return (TSource) (object) value;
        }
    }
}