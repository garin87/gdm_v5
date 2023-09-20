using gdm5._0.Shared.Enums;

namespace gdm5._0.Responses
{
    public class ApplicationResponse
    {
        public ApplicationResponse() { }

        public ApplicationResponse(StatusCodeEnum statusCode) { Code = statusCode; }

        public StatusCodeEnum Code { get; set; } = StatusCodeEnum.Success;

        public static ApplicationResponse Success { get; } = new();
    }
}
