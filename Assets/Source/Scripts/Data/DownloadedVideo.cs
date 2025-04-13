namespace Source.Scripts.Data
{
    public class DownloadedVideo
    {
        public int Id;
        public VideoType VideoType;
        public string Url;

        public DownloadedVideo(int id, VideoType videoType, string url)
        {
            Id = id;
            VideoType = videoType;
            Url = url;
        }
    }
}
