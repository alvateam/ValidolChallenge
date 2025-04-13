using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Source.Scripts.UI
{
    public class TapScreenView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _clicksCountText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private LocalizedString _localizedString;
        [SerializeField] private Image _progressSliderImage;
        [SerializeField] private Image _progressFilledImage;
        [SerializeField] private PointerActionsHandler _pointerActionsHandler;

        private int _videoId;
        private string _imagesNameOriginText;
    
        public event Action Tapped;

        private void OnEnable()
        {
            _localizedString.Arguments = new object[] { _videoId };
            _localizedString.StringChanged += UpdateText;
        }

        private void OnDisable()
        {
            _localizedString.StringChanged -= UpdateText;
        }

        public void Initialize(int videoId)
        {
            _localizedString.Arguments[0] = videoId;
            _localizedString.RefreshString();
        
            SetTapsCount(0);
            UpdateFilledColor();
        }
    
        public void Display()
        {
            _pointerActionsHandler.Clicked += OnTapped;
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            _pointerActionsHandler.Clicked -= OnTapped;
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public void SetTapsCount(int value)
        {
            _clicksCountText.text = value.ToString();
        }

        public void UpdateFilledSlider(float value)
        {
            _progressSliderImage.fillAmount = value;
            UpdateFilledColor();
        }
    
        private void UpdateFilledColor()
        {
            if (_progressSliderImage.fillAmount <= 0.25f)
                _progressFilledImage.color = Color.red;
            else if (_progressSliderImage.fillAmount >= 0.75f)
                _progressFilledImage.color = Color.green;
            else
                _progressFilledImage.color = Color.yellow;
        }
    
        private void OnTapped() => Tapped?.Invoke();
    
        private void UpdateText(string value)
        {
            _nameText.text = value;
        }
    }
}
