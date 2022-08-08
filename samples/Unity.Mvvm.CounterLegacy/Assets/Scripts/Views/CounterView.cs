using UnityMvvmToolkit.Core.Converters.ValueConverters;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UGUI;
using ViewModels;

namespace Views
{
    public class CounterView : CanvasView<CounterViewModel>
    {
        protected override IConverter[] GetValueConverters()
        {
            return new IConverter[] { new IntToStrConverter() };
        }
    }
}