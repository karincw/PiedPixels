namespace RPCServer.RpcService
{
    public interface IRpcMethodBase
    {
        public Task<object> HelloAsync(Dictionary<string, object>? parameters);
    }
}
