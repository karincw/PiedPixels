namespace RPCServer.RpcService
{
    public interface IRpcMethodBase
    {
        public Task<HelloResponse> HelloAsync(Dictionary<string, object>? parameters);
    }

    public record HelloResponse(string message);
}
