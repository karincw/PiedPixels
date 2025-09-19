using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace RPCServer
{
    public class GameHub : Hub
    {
        private readonly GameManager gameManager;

        public GameHub(GameManager manager)
        {
            gameManager = manager;
        }

        // 클라이언트 -> 서버 호출
        public async Task SendMessage(string user, string message)
        {
            // 서버 -> 모든 클라이언트로 전송
            await Clients.Others.SendAsync("ReceiveMessage", user, message);
        }

        public async Task AddPlayer(string userID)
        {
            Player newPlayer = new Player(userID, 0, 0);
            await gameManager.AddPlayer(userID, newPlayer);
        }

        public async Task<List<PlayerPositionDTO>> GetPlayerDatas()
        {
            return await gameManager.GetPlayerDatas();
        }

        public async Task MovePlayer(string userID, int x, int y)
        {
            await gameManager.MovePlayer(userID, x, y);
            await BroadCastMove();
        }

        public async Task BroadCastMove(PlayerPositionDTO playerData)
        {
            await Clients.Others.SendAsync("RecivePlayerData", playerData);
        }

    }
}
