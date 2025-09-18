using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace RPCServer
{
    public class GameHub : Hub
    {
        // 클라이언트 -> 서버 호출
        public async Task SendMessage(string user, string message)
        {
            // 서버 -> 모든 클라이언트로 전송
            await Clients.Others.SendAsync("ReceiveMessage", user, message);
        }
    }
}
