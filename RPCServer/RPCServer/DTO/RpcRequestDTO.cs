namespace RPCServer.DTO
{
    public class RpcRequestDTO
    {
        public string Jsonrpc { get; set; } = "2.0";
        public string? Method { get; set; } = "";
        public Dictionary<string, object>? Params { get; set; }
        public int Id { get; set; }
    }
}
