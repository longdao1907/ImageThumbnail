using ImgThumbnailApp.Web.Models;
using ImgThumbnailApp.Web.Services.IServices;
using ImgThumbnailApp.Web.Utilities;
using System.Text;
using System.Text.Json;

namespace ImgThumbnailApp.Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ResponseDto> SendAsync(RequestDto requestDto)
        {
            HttpClient client = _httpClientFactory.CreateClient("ImgThumbnailAppAPI");
            HttpRequestMessage message = new HttpRequestMessage();

            if (requestDto.ContentType == Utilities.SD.ContentType.MultipartFormData)
            {
                message.Headers.Add("Accept", "*/*");
            }
            else
            {
                message.Headers.Add("Accept", "application/json");
            }




            //token

            message.RequestUri = new Uri(requestDto.Url);

            if (requestDto.ContentType == Utilities.SD.ContentType.MultipartFormData)
            {
                var content = new MultipartFormDataContent();

                foreach (var prop in requestDto.Data.GetType().GetProperties())
                {
                    var value = prop.GetValue(requestDto.Data);
                    if (value is FormFile)
                    {
                        var file = (FormFile)value;
                        if (file != null)
                        {
                            content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                        }
                    }
                    else
                    {
                        content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
                    }
                }
                message.Content = content;
            }
            else
            {
                if (requestDto.Data != null)
                {
                    message.Content = new StringContent(JsonSerializer.Serialize(requestDto.Data), Encoding.UTF8, "application/json");
                }
            }

          

            HttpResponseMessage? apiResponse = null;

            switch (requestDto.ApiType)
            {
                case SD.ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case SD.ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                case SD.ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            apiResponse = await client.SendAsync(message);
            try
            {
                
                switch (apiResponse.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        return new ResponseDto() { IsSuccess = false, Message = "Not Found" };
                    case System.Net.HttpStatusCode.BadRequest:
                        return new ResponseDto() { IsSuccess = false, Message = "Bad Request" };
                    case System.Net.HttpStatusCode.Unauthorized:
                    case System.Net.HttpStatusCode.Forbidden:
                        return new ResponseDto() { IsSuccess = false, Message = "Access Denied" };
                    case System.Net.HttpStatusCode.InternalServerError:
                        return new ResponseDto() { IsSuccess = false, Message = "Internal Server Error" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonSerializer.Deserialize<ResponseDto>(apiContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        return apiResponseDto;
                }
            }
            catch (Exception ex)
            {

                var dto = new ResponseDto
                {
                    IsSuccess = false,
                    Message = ex.Message
                };

                return dto;
            }

        }
    }
}

