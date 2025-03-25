﻿using UnityEngine;
using UnityEngine.Video;

namespace Data
{
    [CreateAssetMenu(fileName = "Static Data", menuName = "VideoData", order = 0)]
    public class VideoData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private VideoType _videoType;
        [SerializeField] private VideoClip _loopedVideo;
        [SerializeField] private VideoClip _finalVideo;
        [SerializeField] private float _energyGain;
        [SerializeField] private float _difficultyMultiplier;
        [SerializeField] private int _idealClicks;
        [SerializeField] private bool _lockedByStarts;
        [SerializeField] private int _starsToUnlock;

        public string ID => _id;
        public VideoType VideoType => _videoType;
        public VideoClip LoopedVideo => _loopedVideo;
        public VideoClip FinalVideo => _finalVideo;
        public float EnergyGain => _energyGain;
        public float DifficultyMultiplier => _difficultyMultiplier;

        public int IdealClicks => _idealClicks;
    }
}