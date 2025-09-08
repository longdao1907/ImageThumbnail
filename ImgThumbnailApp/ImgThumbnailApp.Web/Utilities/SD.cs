namespace ImgThumbnailApp.Web.Utilities
{
    public class SD
    {

        public static string ImageAPIBase { get; set; } 
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
        public enum ContentType
        {
            Json,
            MultipartFormData,
        }
    }
}
