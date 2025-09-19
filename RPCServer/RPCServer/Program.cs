using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace RPCServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<GameManager>();
            builder.Services.AddSignalR();

            var app = builder.Build();

            // 라우팅
            app.MapGet("/", () => "Game Server is running!");
            app.MapHub<GameHub>("/gamehub");  // 허브 엔드포인트

            app.Run();
        }
    }
}
