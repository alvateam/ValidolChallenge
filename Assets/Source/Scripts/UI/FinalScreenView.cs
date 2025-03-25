using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FinalScreenView : MonoBehaviour, IDisposable
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _shareButton;
    [SerializeField] private TextMeshProUGUI _tapCountText;
    [SerializeField] private GradeView[] _gradeViews;
    
    public event Action RestartButtonClicked;

    public void Initialize()
    {
        _restartButton.onClick.AddListener(OnRestartButtonClicked);
    }

    private void OnRestartButtonClicked()
    {
        RestartButtonClicked?.Invoke();
    }

    public void Display()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public void SetTapsCount(int value) => 
        _tapCountText.text = value.ToString();
    
    public void Dispose() => 
        _restartButton.onClick.RemoveListener(OnRestartButtonClicked);

    public void SetGrade(int value)
    {
        for (int i = 0; i < value; i++)
        {
            var view = _gradeViews[i];
            view.ShowGrade();
        }
    }
}
