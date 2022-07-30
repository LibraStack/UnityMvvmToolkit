using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine;
using UnityEngine.UIElements;

namespace LabelAnimations
{
    public class LabelScaleAnimation : ILabelAnimation
    {
        private const string LabelAnimationClassName = "count-label--animation";

        private readonly Label _label;
        private readonly Scale _startScale;
        private readonly Scale _endScale;

        public LabelScaleAnimation(Label label, Vector3 startScale, Vector3 endScale)
        {
            _label = label;
            _startScale = new Scale(startScale);
            _endScale = new Scale(endScale);
        }

        public async UniTask PlayAsync()
        {
            _label.RemoveFromClassList(LabelAnimationClassName);
            _label.style.scale = _startScale;

            await UniTask.Yield();

            _label.AddToClassList(LabelAnimationClassName);
            await UniTask.Yield();
            _label.style.scale = _endScale;
        }
    }
}