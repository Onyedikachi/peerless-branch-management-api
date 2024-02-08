using Newtonsoft.Json;
using Retail.Branch.Core.Common;
using retail_teams_management_api.Exceptions;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace retail_teams_management_api.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(
            RequestDelegate next
             )
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new ApiResponse<object>(error?.Message ?? "An error occured", error?.Message, 400);
                switch (error)
                {
                    //case ApiException e:
                    //    // custom application error
                    //    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    //    break;
                    case ValidationException e:
                        // custom application error
                        responseModel.Status = response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Data = e.Data;
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        responseModel.Status = response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case UnauthorizedAccessException e:
                        responseModel.Status = response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;

                    case PermissionException:
                        responseModel.Status = response.StatusCode = (int)HttpStatusCode.BadRequest;

                        break;
                    default:
                        // unhandled error
                        responseModel.Status = response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        Log.Fatal(error, "An error has occurred ");

                        break;
                }
                var result = JsonConvert.SerializeObject(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}
