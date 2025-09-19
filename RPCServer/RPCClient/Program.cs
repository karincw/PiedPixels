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

            connection.On<PlayerPositionDTO>("RecivePlayerData", (playerdata) =>
            {
                // 현재 입력 줄 지우기
                ClearCurrentConsoleLine();

                Console.WriteLine($"playerMoved : ({playerdata.X}, {playerdata.Y})");

                // 다시 입력 프롬프트 출력
                Console.Write("메시지 입력: ");
            });

            await connection.StartAsync();
            Console.WriteLine("Connected to server!");

            while (true)
            {
                Console.Write("메시지 입력: ");
                var msg = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(msg))
                {
                    await connection.InvokeAsync("SendMessage", userName, $"User Exist {userName}");

                }
                else if (msg == "AddPlayer")
                {
                    await connection.InvokeAsync("AddPlayer", userName);

                    Console.WriteLine("Player Added!!");
                }
                else if(msg == "GetPlayerDatas")
                {
                    List<PlayerPositionDTO> playerdatas =
                        await connection.InvokeAsync<List<PlayerPositionDTO>>("GetPlayerDatas");

                    for (int i = 0; i < playerdatas.Count; i++)
                    {
                        Console.WriteLine($"playerData : {playerdatas[i]}");
                    }
                }
                else
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