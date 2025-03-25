using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputActionsHandler : MonoBehaviour, IPointerClickHandler
{
    public event Action Clicked;
    public event Action RestartButtonClicked;

    [SerializeField] private Button _restartButton;

    private void Awake()
    {
        _restartButton.onClick.AddListener(OnRestartButtonClicked);
    }

    private void OnRestartButtonClicked()
    {
        RestartButtonClicked?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke();
    }
}
