using Microsoft.AspNetCore.SignalR;

namespace RPCServer
{
    public class GameLoopService : BackgroundService
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly GameManager _gameManager;
        private readonly TimeSpan _tickInterval = TimeSpan.FromMilliseconds(500); // ms

        public GameLoopService(IHubContext<GameHub> hubContext, GameManager gameManager)
        {
            _hubContext = hubContext;
            _gameManager = gameManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //틱마다 반복
            while (!stoppingToken.IsCancellationRequested)
            {
                // 전체 게임을 업데이트 하는 함수
                _gameManager.UpdateGame();

                // 모든 플레이어의 현재 상태를 주기적으로 브로드캐스팅
                var allPlayerPositions = await _gameManager.GetPlayerDatas();
                if (allPlayerPositions.Any())
                {
                    //허브에 접근해서 UpdateGameState를 RPC로 실행
                    await _hubContext.Clients.All.SendAsync("UpdatePlayerPositions", allPlayerPositions);
                }

                await Task.Delay(_tickInterval, stoppingToken);
            }
        }
    }
}
