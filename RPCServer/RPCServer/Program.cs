using RPCServer.DTO;
using RPCServer.RpcService;
using System.Text.Json;

namespace RPCServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, Func<Dictionary<string, object>?, Task<object>>> rpcMethodDictionary = new();
            RpcMethod rpcMethod = new();

            var methods = typeof(IRpcMethodBase).GetMethods();
            foreach (var method in methods)
            {
                string rpcName = method.Name.Replace("Async", "").ToLower();

                rpcMethodDictionary[rpcName] = async (parameter) =>
                {
                    var result = method.Invoke(rpcMethod, new object?[] { parameter });
                    return await (Task<object>)result;
                };

            }

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

                //RPC실행
                object? result = null;

                if(rpcRequest.Method == null)
                {
                    return Results.Json(new { error = "Method Name Required" });
                }

                if(rpcMethodDictionary.TryGetValue(rpcRequest.Method.ToLower(), out var handle))
                {
                    result = await handle(rpcRequest.Params);
                }

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
