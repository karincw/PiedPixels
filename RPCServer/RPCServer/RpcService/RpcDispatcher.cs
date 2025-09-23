using RPCServer.DTO;

namespace RPCServer.RpcService
{
    public class RpcDispatcher
    {
        // Key : String
        // Value : Func
        // Func : [ Params : Nullable<Dictionary<string, object>>, Return : Task<object> ]
        private Dictionary<string, Func<Dictionary<string, object>?, Task<object>>> rpcMethodDictionary = new();

        public RpcDispatcher()
        {
            RpcMethod rpcMethod = new();

            var methods = typeof(IRpcMethodBase).GetMethods();
            foreach (var method in methods)
            {
                string rpcName = method.Name.Replace("Async", "").ToLower();

                rpcMethodDictionary[rpcName] = async (parameter) =>
                {
                    var result = method.Invoke(rpcMethod, new object?[] { parameter });
                    if (result is Task task)
                    {
                        await task; // Task 완료까지 기다림

                        // Task<T> 인 경우 결과 꺼내기
                        var taskType = task.GetType();
                        if (taskType.IsGenericType)
                        {
                            return taskType.GetProperty("Result")?.GetValue(task);
                        }
                        return null; // void Task인 경우
                    }

                    // Task가 아닌 경우(동기 메서드라면) 그냥 반환
                    return result;
                };

            }
        }

        public async Task<RpcResponseDTO> DispatchAsync(RpcRequestDTO request)
        {
            RpcResponseDTO responseDTO = new();
            if (rpcMethodDictionary.TryGetValue(request.Method.ToLower(), out var handler))
            {
                responseDTO.id = request.Id;
                try
                {
                    object result = await handler(request.Params);

                    if (result is Response response)
                    {
                        responseDTO.JsonRpc = request.JsonRpc;
                        responseDTO.result = response.result;
                        responseDTO.error = response.error;
                    }
                    else
                    {
                        responseDTO.result = result;
                    }
                }
                catch (Exception ex)
                {
                    responseDTO.error = new RpcErrorDTO(-32000, "Server error", ex.Message);
                }
            }
            else
            {
                responseDTO.error = new RpcErrorDTO(-32601, "Method not found", null);
            }

            return responseDTO;
        }


    }
}
