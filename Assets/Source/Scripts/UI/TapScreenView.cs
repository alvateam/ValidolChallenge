using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TapScreenView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _clicksCountText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _progressSliderImage;
    [SerializeField] private Image _progressFilledImage;
    [SerializeField] private PointerActionsHandler _pointerActionsHandler;

    private string _imagesNameOriginText;
    
    public event Action Tapped;
    
    public void Initialize(int videoNumber)
    {
        _nameText.text += $" {videoNumber}";
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
}
