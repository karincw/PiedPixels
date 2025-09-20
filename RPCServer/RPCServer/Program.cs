using RPCServer.DTO;
using RPCServer.RpcService;
using System.Text.Json;

namespace RPCServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RpcDispatcher rpcDispatcher = new RpcDispatcher();
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet("/", () => "Game Server is running!");

            app.MapPost("rpc", async (HttpRequest request) =>
            {
                //StreamReader알아보기
                using var reader = new StreamReader(request.Body);
                //비동기로 끝까지 읽어오기
                var body = await reader.ReadToEndAsync();

                //Request받은 Json데이터를 지정한 DTO로 역직렬화
                var rpcRequest = JsonSerializer.Deserialize<RpcRequestDTO>(body);

                if(rpcRequest == null)
                {
                    //저정된 포멧에 맞지 않음
                    return Results.Json(new {error = "Invalid request"});
                }

                object? result = null;

                if(rpcRequest.Method == null)
                {
                    return Results.Json(new { error = "Method Name Required" });
                }

                //RPC실행
                await rpcDispatcher.DispatchAsync(rpcRequest);

                //응답 전송
                var response = new RpcResponseDTO
                {
                    id = rpcRequest.Id,
                    result = result
                };

                return Results.Json(response);
            });

            app.Run();
        }
    }
}
