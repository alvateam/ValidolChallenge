using System;
using Data;

[Serializable]
public class VideoJsonData
{
    public string Id;
    public VideoType LoopedVideoType;
    public VideoType FinalVideoType;
    public float EnergyGain;
    public float DifficultyMultiplier;
    public int IdealClicks;
    public bool LockedByStarts;
    public int StarsToUnlock;
    public string LoopedVideoUrl;
    public string FinalVideoUrl;
}