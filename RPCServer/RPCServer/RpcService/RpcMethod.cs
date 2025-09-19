
namespace RPCServer.RpcService
{
    public class RpcMethod : IRpcMethodBase
    {
        public Task<object> HelloAsync(Dictionary<string, object>? parameters)
        {
            //Json데이터 속 Params에 Name을 Key로 가진 데이터를 가져와서 스트링으로 만든 뒤 병합
            var name = parameters?["name"].ToString();
            return Task.FromResult<object>(new { message = $"hello, {name}"! });
        }
    }
}
