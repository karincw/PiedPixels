namespace RPCServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<GameManager>();
            builder.Services.AddSignalR();
            builder.Services.AddHostedService<GameLoopService>();

            var app = builder.Build();

            // 라우팅
            app.MapGet("/", () => "Game Server is running!");
            app.MapHub<GameHub>("/gamehub");  // 허브 엔드포인트

            app.Run();
        }
    }
}
