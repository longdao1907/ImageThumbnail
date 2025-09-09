using ImgThumbnailApp.Web.Services.IServices;
using ImgThumbnailApp.Web.Utilities;

namespace ImgThumbnailApp.Web.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }


        public void ClearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.TokenCookie);

        }

        public string? GetToken()
        {
            string? token = null;
            bool? hasToken = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.TokenCookie, out token);
            return hasToken is true ? token : null;
        }

        public void SetToken(string token)
        {
            var cookieOptions = new CookieOptions
            {                
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
          
            };

            // Lấy HttpContext từ IHttpContextAccessor
            var httpContext = _contextAccessor.HttpContext;

            if (httpContext != null)
            {
                // Gắn cookie vào response với các tùy chọn bảo mật đã thiết lập
                httpContext.Response.Cookies.Append(SD.TokenCookie, token, cookieOptions);
            }
      
        }
    }
}
