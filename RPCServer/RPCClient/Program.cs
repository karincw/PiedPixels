using System.Net.WebSockets;
using System.Text;

namespace RPCClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var ws = new ClientWebSocket();
            await ws.ConnectAsync(new Uri("wss://localhost:5000/ws"), CancellationToken.None);

            var msg = "{\"Method\":\"hello\",\"Params\":{\"name\":\"world\"},\"Id\":1}";
            await ws.SendAsync(Encoding.UTF8.GetBytes(msg), WebSocketMessageType.Text, true, CancellationToken.None);

            var buffer = new byte[1024];
            var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
            Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, result.Count));
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);

            Console.ReadKey();
        }
    }
}