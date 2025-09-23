namespace RPCServer.DTO
{
    public record RpcResponseDTO
    {
        public string? JsonRpc { get; set; } = "2.0";
        public object? result { get; set; }
        public RpcErrorDTO? error { get; set; }
        public int id { get; set; }
    }
}
