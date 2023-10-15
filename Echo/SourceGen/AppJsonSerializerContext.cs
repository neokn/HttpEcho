using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(JsonHttpRequest))]
[JsonSerializable(typeof(RequestData))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}