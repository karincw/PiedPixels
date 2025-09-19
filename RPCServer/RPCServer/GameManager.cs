namespace RPCServer
{
    public class GameManager
    {
        private Dictionary<string, Player> playerDictionary = new Dictionary<string, Player>();

        public Task AddPlayer(string id, Player player)
        {
            if (playerDictionary.ContainsKey(id))
            {
                Console.WriteLine("Error : Dictionary Contains Key");
                return Task.CompletedTask;
            }
            playerDictionary[id] = player;
            return Task.CompletedTask;
        }

        public Task<List<PlayerPositionDTO>> GetPlayerDatas()
        {
            return Task.FromResult(playerDictionary.Values
                .Select(player => new PlayerPositionDTO(player.Id,player.X,player.Y)).ToList());
        }

        public Task MovePlayer(string id, int x, int y)
        {
            if(playerDictionary.TryGetValue(id, out var player))
            {
                player.Move(x, y);
            }
            return Task.CompletedTask;
        }
    }
}
