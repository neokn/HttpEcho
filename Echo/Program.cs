using System.Text.Json;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();
app.Use(async (context, next) =>
{
    using var memoryStream = new MemoryStream();
    await memoryStream.WriteAsync(JsonSerializer.SerializeToUtf8Bytes(new JsonHttpRequest(context.Request), new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    }));
    memoryStream.Seek(0, SeekOrigin.Begin);
    await memoryStream.CopyToAsync(context.Response.Body);
    await next();
});
app.Run();