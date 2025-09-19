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

            //서버가 보낸 RPC를 처리하는
            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {           //   매개변수         RPC콜 이름      매개변수 매칭
                ClearCurrentConsoleLine();
                Console.WriteLine($"{user}: {message}");
            });

            connection.On<PlayerPositionDTO>("RecivePlayerPositionData", (playerdata) =>
            {
                ClearCurrentConsoleLine();
                Console.WriteLine($"{playerdata.Id} Moved : ({playerdata.X}, {playerdata.Y})");
            });

            connection.On<List<PlayerPositionDTO>>("UpdatePlayerPositions", (playerdatas) =>
            {
                ClearCurrentConsoleLine();
                for (int i = 0; i < playerdatas.Count; i++)
                {
                    Console.WriteLine($"{playerdatas[i].Id} Moved : ({playerdatas[i].X}, {playerdatas[i].Y})");
                }
            });

            await connection.StartAsync();
            Console.WriteLine("Connected to server!");
            await OnConnect(connection, userName);

            await UserInput(connection, userName);

        }

        static async Task OnConnect(HubConnection connection, string userName)
        {
            await connection.InvokeAsync("AddPlayer", userName);
        }

        static async Task MovePlayer(HubConnection connection, PlayerPositionDTO playerposition)
        {
            await connection.InvokeAsync("MovePlayer", playerposition.Id, playerposition.X, playerposition.Y);
        }

        static async Task UserInput(HubConnection connection, string userName)
        {
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.W:
                        await MovePlayer(connection, new PlayerPositionDTO(userName, 0, 1));
                        break;
                    case ConsoleKey.A:
                        await MovePlayer(connection, new PlayerPositionDTO(userName, -1, 0));
                        break;
                    case ConsoleKey.S:
                        await MovePlayer(connection, new PlayerPositionDTO(userName, 0, -1));
                        break;
                    case ConsoleKey.D:
                        await MovePlayer(connection, new PlayerPositionDTO(userName, 1, 0));
                        break;
                    case ConsoleKey.Escape:
                        await connection.InvokeAsync("SendMessage", userName, $"User Exist {userName}");
                        await connection.StopAsync();
                        return;
                    default:
                        break;
                }
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