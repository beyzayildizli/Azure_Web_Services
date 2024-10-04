using System.Net;

namespace RestApi;

public class ResultModel<T>
{
    public bool IsSuccess { get; set; }
    public HttpStatusCode Status { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}
