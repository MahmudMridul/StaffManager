using System.Net;

namespace RBAC_API.Models
{
    public class RbacResponse
    {
        public bool Success { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public List<string>? Errors { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        public static RbacResponse Create(HttpStatusCode code, string msg, bool success = false, object? data = null, List<string>? errors = null)
        {
            return new RbacResponse
            {
                Success = success,
                StatusCode = code,
                Message = msg,
                Data = data,
                Errors = errors,
            };
        }

        public static RbacResponse Ok(object? data = null, string message = "")
        {
            return new RbacResponse
            {
                Success = true,
                StatusCode = HttpStatusCode.OK,
                Message = string.IsNullOrEmpty(message) ? "Request was successful" : message,
                Data = data,
                Errors = null
            };
        }

        public static RbacResponse Created(object? data = null, string message = "")
        {
            return new RbacResponse
            {
                Success = true,
                StatusCode = HttpStatusCode.Created,
                Message = string.IsNullOrEmpty(message) ? "Resource created successfully" : message,
                Data = data,
                Errors = null
            };
        }

        public static RbacResponse BadRequest(string msg, List<string>? errors = null)
        {
            return new RbacResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = msg,
                Errors = errors,
            };
        }

        public static RbacResponse ValidationFailed(List<string> errors)
        {
            return new RbacResponse
            {
                Success = false,
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Validation failed",
                Errors = errors
            };
        }

        public static RbacResponse Conflict(string msg)
        {
            return new RbacResponse
            {
                Success = false,
                StatusCode = HttpStatusCode.Conflict,
                Message = msg,
            };
        }

    }
}
