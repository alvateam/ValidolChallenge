using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Source.Scripts.UI.LoadingScreen
{
    public class LoadingScreenView : MonoBehaviour
    {
        private const float FadeDuration = 1f;
    
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _indicator;
        [SerializeField] private float _indicatorRotatinDuration = 2f;
    
        private TweenerCore<Quaternion, Vector3, QuaternionOptions> _rotationTween;

        private void Start()
        {
            _rotationTween = _indicator.DORotate(new Vector3(0, 0, -360), _indicatorRotatinDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }

        public void Hide()
        {
            _canvasGroup
                .DOFade(0, FadeDuration)
                .OnComplete(() =>
                {
                    _canvasGroup.blocksRaycasts = false;
                    _canvasGroup.interactable = false;
            
                    _rotationTween.Kill();
                    _rotationTween = null;
                });
        }
    }
}
