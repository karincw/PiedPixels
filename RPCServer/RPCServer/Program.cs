using RPCServer.DTO;
using RPCServer.RpcService;
using RPCServer.Serializer;
using System.Buffers;
using System.Net.WebSockets;

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
                // 현재 요청이 WebSocket 요청인지 검사
                if (!context.WebSockets.IsWebSocketRequest)
                {
                    // WS 요청이 아니면 400(Bad Request) 반환
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                // WS 핸드셰이크 수락 (여기서부터 연결이 성립됨)
                // 핸드셰이크 : http에서 websocket으로 업그레이드
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                byte[] buffer = null;

                // 연결 열린 동안 루프
                // 각각의 webSocket마다 하나씩 돌아감
                // 이벤트? 비슷하게 돌아가기 때문에 데이터를 받을때까지 잠들어있음
                // 컨택스드 스위칭 안일어남
                while (webSocket.State == WebSocketState.Open)
                {
                    try
                    {
                        //ArrayPool에서 버퍼 빌려오기
                        buffer = ArrayPool<byte>.Shared.Rent(1024 * 4);

                        // WEbSocket이 하나의 메세지를 여러버퍼에 나눠서 보낼수 있기떄문에 MemoryStream을 이용해서 이어붙여야함    
                        using var memoryStream = new MemoryStream();

                        WebSocketReceiveResult receiveResult;
                        do
                        {
                            // 프레임 수신(부분 수신 가능)
                            //    - result.Count: 이번 프레임에 담긴 바이트 수
                            //    - result.EndOfMessage: 이 프레임이 메시지의 끝인지 여부
                            //    - result.MessageType: Text/Binary/Close 중 하나
                            receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                            // 클라이언트가 종료 의사를 보낸 경우
                            if (receiveResult.MessageType == WebSocketMessageType.Close)
                            {
                                // 정상 종료 응답을 보내고 종료
                                await webSocket.CloseAsync(
                                    WebSocketCloseStatus.NormalClosure,
                                    "Closed by client",
                                    CancellationToken.None);

                                return; // 연결 종료
                                // return시에도 finally는 작동함
                            }

                            memoryStream.Write(buffer, 0, receiveResult.Count);

                        } while (!receiveResult.EndOfMessage); // 메시지가 끝날때까지 반복

                        // 텍스트 메시지만 처리 (바이너리는 스킵하거나 별도 처리 로직 추가 가능)
                        if (receiveResult.MessageType != WebSocketMessageType.Text)
                        {
                            // 필요시 Binary도 허용하려면 여기서 분기 처리
                            continue;
                        }

                        DataFormat inputFormat = FormatUtility.ResolveInput(context);
                        DataFormat outputFormat = FormatUtility.ResolveOutput(context);

                        RpcRequestDTO rpcRequest = SerializerUtility.DeSerializer(memoryStream.ToArray(), inputFormat);

                        RpcResponseDTO rpcResponse = await rpcDispatcher.DispatchAsync(rpcRequest);


                        var responseBytes = SerializerUtility.Serializer(rpcResponse, outputFormat);

                        // endOfMessage : true 여서 한번에 데이터를 보내지만 네트워크단계에서 나눠서 보내질 수 있음 
                        await webSocket.SendAsync(
                            new ArraySegment<byte>(responseBytes),
                            WebSocketMessageType.Text,
                            endOfMessage: true,
                            cancellationToken: CancellationToken.None);
                    }
                    catch (WebSocketException wsex)
                    {
                        // 네트워크/프로토콜 오류(연결 끊김 등)
                        Console.WriteLine($"[WebSocketException] {wsex.Message}");
                        break;
                    }
                    catch (Exception ex)
                    {
                        // 그 외 예외(핸들러/직렬화/디스패치 오류)
                        Console.WriteLine($"[Unhandled] {ex.Message}");
                        break;
                    }
                    finally
                    {
                        ArrayPool<byte>.Shared.Return(buffer, true);
                    }
                }
            });

            #endregion

            app.Run();
        }
    }
}