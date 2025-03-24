using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _clicksText;
    [SerializeField] private TextMeshProUGUI _imageCountText;
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private Image _progressImage;
    
    [SerializeField] private FilledProgressHandler _filledProgressHandler;
    [SerializeField] private ClicksCounter _clicksCounter;

    private string _clicksOriginText;
    private string _imagesOriginText;
    
    public void Initialize(int videoNumber)
    {
        _clicksOriginText = _clicksText.text;
        _imagesOriginText = _imageCountText.text;
        _clicksCounter.Changed += OnClicksChanged;
        OnClicksChanged(0);
        UpdateFilledColor();
        _filledProgressHandler.ProgressChanged += OnFilledProgressChanged;
        _imageCountText.text = $"{_imagesOriginText} {videoNumber}";
    }

    private void OnFilledProgressChanged(float value)
    {
        _progressSlider.value = value;
        UpdateFilledColor();
    }

    private void OnClicksChanged(int value)
    {
        _clicksText.text = $"{_clicksOriginText} {value}";
    }

    private void UpdateFilledColor()
    {
        if (_progressSlider.value <= 0.25f)
            _progressImage.color = Color.red;
        else if (_progressSlider.value >= 0.75f)
            _progressImage.color = Color.green;
        else
            _progressImage.color = Color.yellow;
    }
}