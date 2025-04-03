namespace _014_Websocket
{
    using System;
    using System.Collections.Concurrent;
    using System.IdentityModel.Tokens.Jwt;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class WebSocketHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// 线程安全字典
        /// </summary>
        private static ConcurrentDictionary<string, WebSocket> _clients = new ConcurrentDictionary<string, WebSocket>();

        public WebSocketHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    // 获取 JWT Token
                    var token = context.Request.Query["token"];
                    if (string.IsNullOrEmpty(token) || !ValidateToken(token, out var userId))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized");
                        return;
                    }

                    //建立Websocket
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await HandleWebSocket(webSocket, userId);
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await _next(context);
            }
        }
        private bool ValidateToken(string token, out string userId)
        {
            userId = null;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                return !string.IsNullOrEmpty(userId);
            }
            catch
            {
                return false;
            }
        }
        private async Task HandleWebSocket(WebSocket webSocket, string userId)
        {

            _clients[userId] = webSocket;

            while (webSocket.State == WebSocketState.Open)
            {
                var buffer = new byte[1024 * 4];
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _clients.TryRemove(userId, out _);
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        break;
                    }

                    var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var responseMessage = $"User {userId} sent: {receivedMessage}";
                    var responseBytes = Encoding.UTF8.GetBytes(responseMessage);
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        
        }

        public static async Task SendMessageToClient(string userId, string message)
        {
            if (_clients.TryGetValue(userId, out var webSocket) && webSocket.State == WebSocketState.Open)
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

}
