using Microsoft.AspNetCore.Components;
using RPCServer.DTO;
using RPCServer.RpcService;
using RPCServer.Serializer;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace RPCServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<RpcDispatcher>();

            var app = builder.Build();

            app.UseWebSockets();

            app.MapGet("/", () => "Game Server is running!");

            #region Http 방식

            app.MapPost("rpc", async (HttpRequest request, RpcDispatcher rpcDispatcher) =>
            {
                //StreamReader알아보기
                using var reader = new StreamReader(request.Body);
                //비동기로 끝까지 읽어오기
                var body = await reader.ReadToEndAsync();

                //입력받은 데이터의 포멧을 확인
                DataFormat inputFormat = FormatUtility.ResolveInput(request);
                DataFormat outputFormat = FormatUtility.ResolveOutput(request);

                //입력받은 데티어를 확인한 포멧으로 역직렬화
                RpcRequestDTO rpcRequest = SerializerUtility.DeSerializer(body, inputFormat);

                if (rpcRequest == null)
                {
                    return Results.Json(new { error = "Invalid request" });
                }

                if (rpcRequest.Method == null)
                {
                    return Results.Json(new { error = "Method Name Required" });
                }

                // RPC실행
                RpcResponseDTO responseDTO = await rpcDispatcher.DispatchAsync(rpcRequest);

                //직렬화
                byte[] response = SerializerUtility.Serializer(responseDTO, outputFormat);

                //전송
                return Results.Bytes(
                    contents: response,
                    contentType: FormatUtility.GetContentType(outputFormat)
                );
            });

            #endregion

            #region WebSocket방식

            app.MapGet("/ws", async (HttpContext context, RpcDispatcher rpcDispatcher) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                    var buffer = new byte[1024 * 4];
                    var cancel = CancellationToken.None;

                    while (true)
                    {
                        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancel);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cancel);
                            break;
                        }

                        var requestJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var rpcRequest = JsonSerializer.Deserialize<RpcRequestDTO>(requestJson);

                        if (rpcRequest != null)
                        {
                            var response = await rpcDispatcher.DispatchAsync(rpcRequest);
                            var responseJson = JsonSerializer.Serialize(response);
                            var responseBytes = Encoding.UTF8.GetBytes(responseJson);

                            await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, cancel);
                        }
                    }
                    Console.WriteLine("Recived!!");
                }
                else
                {
                    context.Response.StatusCode = 400; // WebSocket 요청이 아닌 경우
                }

            });

            #endregion

            app.Run();
        }
    }
}