
namespace RPCServer.RpcService
{
    public class RpcMethod : IRpcMethodBase
    {
        public Task<Response> AddAsync(Dictionary<string, object>? parameters)
        {
            if(!ParameterCheck(parameters, "left"))
            {
                return Task.FromResult(new Response(result: $"Error!!", error: "Parameter does not exist : [left]"));
            }
            if (!ParameterCheck(parameters, "right"))
            {
                return Task.FromResult(new Response(result: $"Error!!", error: "Parameter does not exist : [right]"));
            }

            int left = Convert.ToInt32(parameters?["left"]);
            int right = Convert.ToInt32(parameters?["right"]);
            return Task.FromResult(new Response(result: left + right, error: ""));
        }

        public Task<Response> HelloAsync(Dictionary<string, object>? parameters)
        {
            if (!ParameterCheck(parameters, "name"))
            {
                return Task.FromResult(new Response(result: $"Error!!", error: "Parameter does not exist : [name]"));
            }

            //Json데이터 속 Params에 Name을 Key로 가진 데이터를 가져와서 스트링으로 만든 뒤 병합
            var name = parameters?["name"].ToString();
            return Task.FromResult(new Response(result: $"hello, {name}!", error: ""));
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
