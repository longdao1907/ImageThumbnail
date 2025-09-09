namespace ImgThumbnailApp.Web.Utilities
{
    public class SD
    {

        public static string ImageAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }
        public const string TokenCookie = "JWTToken";
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
