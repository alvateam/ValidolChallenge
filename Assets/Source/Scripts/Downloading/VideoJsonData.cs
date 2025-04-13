using System;
using Source.Scripts.Data;

namespace Source.Scripts.Downloading
{
    [Serializable]
    public class VideoJsonData
    {
        public int Id;
        public VideoType LoopedVideoType;
        public VideoType FinalVideoType;
        public float EnergyGain;
        public float DifficultyMultiplier;
        public int IdealClicks;
        public bool LockedByStarts;
        public int StarsToUnlock;
        public string LoopedVideoId;
        public string FinalVideoId;
    }
}