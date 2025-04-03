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
                var serverUri = new Uri("ws://localhost:5037/ws?token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJkYXRhIjpbeyJ0b29sdHQiOiJodHRwczovL3Rvb2x0dC5jb20ifV0sImlhdCI6MTc0MjI4NzY3OSwiZXhwIjoxOTYzMTUxOTk5LCJhdWQiOiIiLCJpc3MiOiIiLCJzdWIiOiJodWRpbmd3ZW4ifQ.E9T5CBT0PSs-Vy37HxgHj6hx-K9_Uwi0RmafCGOuOO8"); // �滻Ϊ��� WebSocket ��������ַ

                try
                {
                    // ���� WebSocket ������
                    await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);
                    Console.WriteLine("�����ӵ� WebSocket ������");

                    // ������Ϣ
                    string message = content;
                    await SendMessageAsync(clientWebSocket, message);

                    // ���շ���������Ϣ
                    await ReceiveMessageAsync(clientWebSocket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"WebSocket ����ʧ��: {ex.Message}");
                }
                finally
                {
                    // �ر� WebSocket ����
                    await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "�ͻ��˹ر�����", CancellationToken.None);
                    Console.WriteLine("WebSocket �����ѹر�");
                }
            }

            return "OK";

        }


        /// <summary>
        /// ������Ϣ�� WebSocket ������
        /// </summary>
        static async Task SendMessageAsync(ClientWebSocket webSocket, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine($"�ͻ����ѷ�����Ϣ: {message}");
        }

        /// <summary>
        /// ���� WebSocket ����������Ϣ
        /// </summary>
        static async Task ReceiveMessageAsync(ClientWebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("����������ر�����");
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "�������ر�����", CancellationToken.None);
                    break;
                }

                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"�ͻ����յ���������Ϣ: {receivedMessage}");
            }
        }



    }
}
