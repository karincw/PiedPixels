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

        // 클라이언트에서 서버로 호출하는 RPC함수들

        public async Task SendMessage(string user, string message)
        {
            //클라이언트에게 보내는 RPC함수
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
            PlayerPositionDTO playerData = await gameManager.GetPlayerData(userID);
            await BroadCastMove(playerData);
        }

        private async Task BroadCastMove(PlayerPositionDTO playerData)
        {
            await Clients.All.SendAsync("RecivePlayerPositionData", playerData);
        }
        private async Task BroadCastMove(List<PlayerPositionDTO> playerData)
        {
            await Clients.All.SendAsync("RecivePlayerPositionDatas", playerData);
        }

    }
}
