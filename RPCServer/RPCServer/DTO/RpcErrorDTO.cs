namespace RPCServer.DTO
{
    public record RpcErrorDTO(int code, string message, object? data);
}

