namespace JimBobBennett.JimLib.Network
{
    public class RestResponse<T>
    {
        public RestResponse(string message, int statusCode, T responseObject)
        {
            Message = message;
            StatusCode = statusCode;
            ResponseObject = responseObject;
        }

        public string Message { get; private set; }
        public int StatusCode { get; private set; }
        public T ResponseObject { get; private set; }
    }
}