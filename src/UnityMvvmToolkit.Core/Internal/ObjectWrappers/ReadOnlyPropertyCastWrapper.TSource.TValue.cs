namespace UnityMvvmToolkit.Core.Internal.ObjectWrappers
{
    internal sealed class ReadOnlyPropertyCastWrapper<TSource, TValue> : ReadOnlyPropertyWrapper<TSource, TValue>
    {
        public ReadOnlyPropertyCastWrapper() : base(default)
        {
        }

        protected override TValue Convert(TSource value)
        {
            return (TValue) (object) value;
        }
    }
}