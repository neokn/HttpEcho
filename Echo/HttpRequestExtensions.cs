using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

static class HttpRequestExtensions
{
    public static bool TryGetMediaType(this HttpRequest request,
        [NotNullWhen(true)] out MediaTypeHeaderValue? mediaTypeHeaderValue)
    {
        mediaTypeHeaderValue = null;
        return request.Headers.TryGetValue(HeaderNames.ContentType, out var contentType) &&
               MediaTypeHeaderValue.TryParse(contentType.First(), out mediaTypeHeaderValue);
    }

    public static Encoding GetEncoding(this HttpRequest request)
    {
        return request.GetEncoding(Encoding.Default);
    }

    public static Encoding GetEncoding(this HttpRequest request, Encoding defaultEncoding)
    {
        if (request.TryGetMediaType(out var mediaTypeHeaderValue) && mediaTypeHeaderValue.Charset.HasValue &&
            mediaTypeHeaderValue.Encoding != null)
        {
            return mediaTypeHeaderValue.Encoding;
        }

        return defaultEncoding;
    }

    public static Task<string> GetBodyStringAsync(this HttpRequest request)
    {
        return request.GetBodyStringAsync(Encoding.Default);
    }

    public static async Task<string> GetBodyStringAsync(this HttpRequest request, Encoding defaultEncoding)
    {
        request.EnableBuffering();
        request.Body.Seek(0, SeekOrigin.Begin);
        return request.GetEncoding(defaultEncoding).GetString((await request.BodyReader.ReadAsync()).Buffer);
    }

    public static async Task<object?> GetBodyDataAsync(this HttpRequest request)
    {
        if (!request.TryGetMediaType(out var contentMediaType))
        {
            return null;
        }

        request.EnableBuffering();
        request.Body.Seek(0, SeekOrigin.Begin);

        if (contentMediaType.MatchesMediaType(MediaTypeNames.Application.Json))
        {
            return await JsonSerializer.DeserializeAsync<JsonElement>(request.Body);
        }

        if (contentMediaType.MatchesMediaType(MediaTypeNames.Application.FormUrlEncoded))
        {
            var formBody = HttpUtility.ParseQueryString(await request.GetBodyStringAsync());
            return formBody.AllKeys.Select(key =>
                new KeyValuePair<string, StringValues>(key!,
                    new StringValues(formBody.GetValues(key)))).ToList();
        }

        return null;
    }
}