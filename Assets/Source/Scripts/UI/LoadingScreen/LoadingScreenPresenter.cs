using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Scripts.UI.LoadingScreen
{
    public class LoadingScreenPresenter : MonoBehaviour
    {
        [SerializeField] private LoadingScreenView _view;
        [SerializeField] private VideosFactory _videosFactory;
        [SerializeField] private PointerActionsHandler _pointerActionsHandler;
        [SerializeField] private Image _logoImage;
        [SerializeField] private Sprite _logoClickedSprite;
        [SerializeField] private AudioService _audioService;
    
        [SerializeField] private Vector3 _logoScale = new(1.1f, 1.1f, 1.1f);
        [SerializeField] private float _logoScaleDuration = 0.2f;
    
        private Sprite _logoOriginalSprite;
        private Tween _clickFirstPartTween;
        private Tween _clickSecondPartTween;

        private void Awake()
        {
            _logoOriginalSprite = _logoImage.sprite;
            _videosFactory.VideoContainersCreated += OnVideoContainersCreated;
            _pointerActionsHandler.Clicked += OnPointerClicked;
        }

        private void OnPointerClicked()
        {
            _audioService.PlayClick();

            if (_clickFirstPartTween != null)
            {
                _clickFirstPartTween.Complete();
                PlayClickSecondPartAnimation();
            }
            else if (_clickSecondPartTween != null)
            {
                _clickSecondPartTween.Complete();
                PlayClickFirstPartAnimation();
            }
            else
            {
                PlayClickFirstPartAnimation();
            }
        }

        private void PlayClickFirstPartAnimation()
        {
            _clickFirstPartTween = _logoImage.transform.DOScale(_logoScale, _logoScaleDuration).OnComplete(()=>
            {
                _clickFirstPartTween.Kill();
                _clickFirstPartTween = null;
            
                _logoImage.sprite = _logoClickedSprite;
                PlayClickSecondPartAnimation();
            });
        }

        private void PlayClickSecondPartAnimation()
        {
            _clickSecondPartTween = _logoImage.transform.DOScale(Vector3.one, _logoScaleDuration).OnComplete(()=>
            {
                _logoImage.sprite = _logoOriginalSprite;
                _clickSecondPartTween.Kill();
                _clickSecondPartTween = null;
            });
        }

        private void OnVideoContainersCreated(IReadOnlyList<VideoBootstrapper> obj)
        {
            _view.Hide();
        }
    }
}
