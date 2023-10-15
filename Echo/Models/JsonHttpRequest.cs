using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;

class JsonHttpRequest
{
    private readonly HttpRequest _request;
    public string Url => _request.GetDisplayUrl();
    public string Method => _request.Method;
    public string PathAndQuery => _request.GetEncodedPathAndQuery();
    public string Protocol => _request.Protocol;

    public Dictionary<string, string> Headers => 
        _request.Headers.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToString())).ToDictionary();

    public string Body { get; init; }
    public RequestData RequestData => GetData();

    private RequestData GetData()
    {
        return new RequestData
        {
            Query = _request.Query.ToDictionary(),
            ParseBody = GetBodyData()
        };
    }

    private object? GetBodyData()
    {
        try
        {
            return _request.GetBodyDataAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public JsonHttpRequest(HttpRequest request)
    {
        _request = request;
        Body = _request.GetBodyStringAsync(Encoding.UTF8).GetAwaiter().GetResult();
    }
}

class RequestData
{
    public Dictionary<string, StringValues> Query { get; init; }
    public object? ParseBody { get; init; }
}