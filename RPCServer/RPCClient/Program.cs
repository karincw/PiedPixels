using Microsoft.AspNetCore.SignalR.Client;

namespace RPCClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string userName = $"User_{Guid.NewGuid().ToString()[..5]}";
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/gamehub")
                .Build();

            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                // 현재 입력 줄 지우기
                ClearCurrentConsoleLine();

                if (user == userName)
                    Console.WriteLine($"[나] {message}");
                else
                    Console.WriteLine($"{user}: {message}");

                // 다시 입력 프롬프트 출력
                Console.Write("메시지 입력: ");
            });

            await connection.StartAsync();
            Console.WriteLine("Connected to server!");

            while (true)
            {
                Console.Write("메시지 입력: ");
                var msg = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(msg) || msg == "exit")
                {
                    await connection.InvokeAsync("SendMessage", userName, $"User Exist {userName}");
                }

                await connection.InvokeAsync("SendMessage", userName, msg);
            }
        }

        static void ClearCurrentConsoleLine()
        {
            int currentLine = Console.CursorTop;
            Console.SetCursorPosition(0, currentLine);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLine);
        }
    }
}