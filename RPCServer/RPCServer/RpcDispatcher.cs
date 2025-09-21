using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using RPCServer.DTO;
using RPCServer.RpcService;

namespace RPCServer
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
            RpcResponseDTO resultDTO = new();
            if (rpcMethodDictionary.TryGetValue(request.Method.ToLower(), out var handler))
            {
                resultDTO.id = request.Id;
                try
                {
                    var result = await handler(request.Params);
                    resultDTO.result = result;
                }
                catch (Exception ex)
                {
                    resultDTO.error = ex.Message;
                }
                return resultDTO;
            }
            else
            {
                resultDTO.id = request.Id;
                resultDTO.error = "Method Not Found";
                return resultDTO;
            }
        }


    }
}
