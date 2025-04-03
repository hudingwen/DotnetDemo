using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace _014_Websocket.Controllers
{
    [ApiController]
    [Route("client")]
    public class ClientController : ControllerBase
    { 

        [HttpGet]
        [Route("test")]
        public async Task<string> GetAsync(string content)
        {

            using (var clientWebSocket = new ClientWebSocket())
            {
                var serverUri = new Uri("ws://localhost:5037/ws?token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJkYXRhIjpbeyJ0b29sdHQiOiJodHRwczovL3Rvb2x0dC5jb20ifV0sImlhdCI6MTc0MjI4NzY3OSwiZXhwIjoxOTYzMTUxOTk5LCJhdWQiOiIiLCJpc3MiOiIiLCJzdWIiOiJodWRpbmd3ZW4ifQ.E9T5CBT0PSs-Vy37HxgHj6hx-K9_Uwi0RmafCGOuOO8"); // 替换为你的 WebSocket 服务器地址

                try
                {
                    // 连接 WebSocket 服务器
                    await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);
                    Console.WriteLine("已连接到 WebSocket 服务器");

                    // 发送消息
                    string message = content;
                    await SendMessageAsync(clientWebSocket, message);

                    // 接收服务器的消息
                    await ReceiveMessageAsync(clientWebSocket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"WebSocket 连接失败: {ex.Message}");
                }
                finally
                {
                    // 关闭 WebSocket 连接
                    await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "客户端关闭连接", CancellationToken.None);
                    Console.WriteLine("WebSocket 连接已关闭");
                }
            }

            return "OK";

        }


        /// <summary>
        /// 发送消息到 WebSocket 服务器
        /// </summary>
        static async Task SendMessageAsync(ClientWebSocket webSocket, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine($"客户端已发送消息: {message}");
        }

        /// <summary>
        /// 接收 WebSocket 服务器的消息
        /// </summary>
        static async Task ReceiveMessageAsync(ClientWebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("服务器请求关闭连接");
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "服务器关闭连接", CancellationToken.None);
                    break;
                }

                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"客户端收到服务器消息: {receivedMessage}");
            }
        }



    }
}
