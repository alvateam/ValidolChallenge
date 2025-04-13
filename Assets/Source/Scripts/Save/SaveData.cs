using System;

namespace Source.Scripts.Save
{
    [Serializable]
    public class SaveData
    {
        public int VideoId;

        public SaveData()
        {
            VideoId = 1;
        }
    }
}