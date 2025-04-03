using System.Net;
using System.Net.Sockets;
using System.Text;

namespace _015_Socket
{
    public class SocketHelper
    {
        public static async Task CreateServer()
        {
            int port = 5000; // 监听的端口
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine($"服务器启动，监听端口 {port}...");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync(); // 等待客户端连接
                Console.WriteLine("客户端已连接");

                _ = Task.Run(() => HandleClient(client)); // 处理客户端
            }
        }
        public static async Task HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // 连接关闭

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"收到客户端消息: {message}");

                // 发送回显消息
                string response = $"服务器返回消息: {message}";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }

            client.Close();
            Console.WriteLine("客户端断开连接");
        }
         
        public static async Task<string> CreateClient(string msg )
        {
            string serverIp = "127.0.0.1"; // 服务器 IP
            int port = 5000; // 服务器端口

            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync(serverIp, port);
                Console.WriteLine("已连接到服务器");

                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                 
                byte[] data = Encoding.UTF8.GetBytes(msg);
                await stream.WriteAsync(data, 0, data.Length);

                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"客户端收到服务端消息: {response}");
                return response;
                //while (true)
                //{
                //    Console.Write("输入消息: ");
                //    string message = Console.ReadLine();
                //    if (string.IsNullOrEmpty(message)) break;

                //    byte[] data = Encoding.UTF8.GetBytes(message);
                //    await stream.WriteAsync(data, 0, data.Length);

                //    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                //    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                //    Console.WriteLine($"服务器响应: {response}");
                //}
            }
            Console.WriteLine("客户端已断开");
        }
    }
}
