

namespace ImgThumbnailApp.Web.Models
{
    public class RequestDto
    {
        public Utilities.SD.ApiType ApiType { get; set; } = Utilities.SD.ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string AccessToken { get; set; }
        public Utilities.SD.ContentType ContentType { get; set; } = Utilities.SD.ContentType.Json;
    }
}
    