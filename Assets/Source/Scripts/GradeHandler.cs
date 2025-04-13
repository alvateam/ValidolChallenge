using System;
using UnityEngine;

namespace Source.Scripts
{
    public class GradeHandler : MonoBehaviour
    {
        [SerializeField] private TapsCounter _tapsCounter;
        [SerializeField] private VideoPlayerWrapper _videoPlayer;
    
        private int _idealClicks;
    
        public event Action<int> GradeCalculated;

        public void Initialize(int idealClicks)
        {
            _idealClicks = idealClicks;
            _videoPlayer.FinalVideoFinished += OnFinalVideoFinished;
        }

        private void OnFinalVideoFinished()
        {
            CalculateGrade();
        }

        private void CalculateGrade()
        {
            int grade = 1;
            var currentClicks = _tapsCounter.Count;
        
            if (currentClicks <= _idealClicks)
                grade = 3;
            else if (currentClicks <= _idealClicks * 1.25f)
                grade = 2;

            GradeCalculated?.Invoke(grade);
        }
    }
}
