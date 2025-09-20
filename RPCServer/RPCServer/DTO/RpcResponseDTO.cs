namespace RPCServer.DTO
{
    public class RpcResponseDTO
    {
        public object? result { get; set; }
        public object? error { get; set; }
        public int id { get; set; }
    }
}
