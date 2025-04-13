using UnityEngine;

namespace Source.Scripts.UI
{
    public class GradeView : MonoBehaviour
    {
        [SerializeField] private GameObject _filledGrade;

        public void ShowGrade()
        {
            _filledGrade.SetActive(true);
        }

        public void HideGrade()
        {
            _filledGrade.SetActive(false);
        }
    }
}
