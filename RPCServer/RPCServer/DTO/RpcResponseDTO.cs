namespace RPCServer.DTO
{
    public class RpcResponseDTO
    {
        public string Jsonrpc { get; set; } = "2.0";
        public object? result { get; set; }
        public object? error { get; set; }
        public int id { get; set; }
    }
}
