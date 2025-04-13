using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Source.Scripts
{
    public class PointerActionsHandler : MonoBehaviour, IPointerClickHandler
    {
        public event Action Clicked;

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke();
        }
    }
}
