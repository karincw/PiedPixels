using RPCServer.DTO;

namespace RPCServer.RpcService
{
    public interface IRpcMethodBase
    {
        public Task<Response> HelloAsync(Dictionary<string, object>? parameters);
        public Task<Response> AddAsync(Dictionary<string, object>? parameters);
    }

    public record Response(object result, RpcErrorDTO error);
}
