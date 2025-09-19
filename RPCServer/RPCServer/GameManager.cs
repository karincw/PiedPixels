using System.Collections.Concurrent;

namespace RPCServer
{
    public class GameManager
    {
        private ConcurrentDictionary<string, Player> playerDictionary = new ConcurrentDictionary<string, Player>();

        public Task AddPlayer(string id, Player player)
        {
            playerDictionary[id] = player;
            return Task.CompletedTask;
        }

        public Task RemovePlayer(string id)
        {
            playerDictionary.TryRemove(id, out var player);
            return Task.CompletedTask;
        }

        public Task<List<PlayerPositionDTO>> GetPlayerDatas()
        {
            return Task.FromResult(playerDictionary.Values
                .Select(player => new PlayerPositionDTO(player.Id, player.X, player.Y)).ToList());
        }

        public Task<PlayerPositionDTO> GetPlayerData(string id)
        {
            return Task.FromResult<PlayerPositionDTO>(new PlayerPositionDTO(id, playerDictionary[id].X, playerDictionary[id].Y));
        }

        public Task MovePlayer(string id, int x, int y)
        {
            if (playerDictionary.TryGetValue(id, out var player))
            {
                player.Move(x, y);
            }
            return Task.CompletedTask;
        }

        public void UpdateGame()
        {

        }
    }
}
