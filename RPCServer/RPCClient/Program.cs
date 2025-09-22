using System.Net.WebSockets;
using System.Text;

namespace RPCClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var ws = new ClientWebSocket();
            await ws.ConnectAsync(new Uri("ws://localhost:5000/ws?in=yaml&out=json"), CancellationToken.None);
            
            Console.WriteLine("Server Connect");

            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                //var msg = "{\"Method\":\"hello\",\"Params\":{\"name\":\"world\"},\"Id\":1}";
                var msg = "Method: hello\nParams:\n  name: world\nId: 1";
                await ws.SendAsync(Encoding.UTF8.GetBytes(msg), WebSocketMessageType.Text, true, CancellationToken.None);

                var buffer = new byte[1024];
                var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
                Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, result.Count));
            }

            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);

            Console.ReadKey();
        }
    }
}