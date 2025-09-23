
using RPCServer.DTO;

namespace RPCServer.RpcService
{
    public class RpcMethod : IRpcMethodBase
    {
        public Task<Response> AddAsync(Dictionary<string, object>? parameters)
        {
            string leftName = "left", rightName = "right";
            if (!ParameterCheck(parameters, leftName))
            {
                return Task.FromResult(
                    new Response(result: $"Error!!", error: new RpcErrorDTO(-32602, $"Invalid params : [{leftName}]", null)));
            }
            if (!ParameterCheck(parameters, rightName))
            {
                return Task.FromResult(new Response(result: $"Error!!", error: new RpcErrorDTO(-32602, $"Invalid params : [{rightName}]", null)));
            }

            int left = Convert.ToInt32(parameters?[leftName]);
            int right = Convert.ToInt32(parameters?[rightName]);
            return Task.FromResult(new Response(result: left + right, error: null));
        }

        public Task<Response> HelloAsync(Dictionary<string, object>? parameters)
        {
            string paramName = "name";
            if (!ParameterCheck(parameters, paramName))
            {
                return Task.FromResult(new Response(result: $"Error!!", error: new RpcErrorDTO(-32602, $"Invalid params : [{paramName}]", null)));
            }

            //Json데이터 속 Params에 Name을 Key로 가진 데이터를 가져와서 스트링으로 만든 뒤 병합
            var name = parameters?["name"].ToString();
            return Task.FromResult(new Response(result: $"hello, {name}!", error: null));
        }

        private static bool ParameterCheck(Dictionary<string, object>? parameters, string find)
        {
            if (parameters == null || !parameters.ContainsKey(find) || parameters[find] == null)
            {
                return false;
            }
            return true;
        }
    }
}
