using Microsoft.AspNetCore.Http;
using Serilog;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. 记录请求内容
        await LogRequest(context);

        // 2. 记录响应内容
        await LogResponse(context);
    }

    private async Task LogRequest(HttpContext context)
    {
        context.Request.EnableBuffering(); // 允许重复读取请求流

        var request = context.Request;
        var requestBody = await ReadStreamAsStringAsync(request.Body);

        // 记录请求方法、路径、查询参数和正文
        Log.Information("HTTP Request Information: Method={Method}, Path={Path}, QueryString={QueryString}, Body={Body}",
            request.Method,
            request.Path,
            request.QueryString,
            requestBody);

        // 由于流已被读取，需要将其重置回开始位置，以便后续中间件使用
        request.Body.Position = 0;
    }

    private async Task LogResponse(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;

        // 使用临时的内存流捕获响应
        using (var responseBody = new MemoryStream())
        {
            context.Response.Body = responseBody;

            // 调用下一个中间件，生成响应
            await _next(context);

            // 读取响应内容
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBodyContent = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            // 记录响应状态码和正文
            Log.Information("HTTP Response Information: StatusCode={StatusCode}, Body={Body}",
                context.Response.StatusCode,
                responseBodyContent);

            // 将响应内容写回原始响应流
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private async Task<string> ReadStreamAsStringAsync(Stream stream)
    {
        // 检查流是否为空，避免异常
        if (stream == null || !stream.CanRead)
        {
            return string.Empty;
        }

        using (var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true))
        {
            return await reader.ReadToEndAsync();
        }
    }
}
