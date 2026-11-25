namespace HealthGuardRwanda.Core.Controllers;

public sealed class ServiceResult
{
    public int Status { get; init; }
    public string Message { get; init; } = string.Empty;
    public object? Data { get; init; }
    public bool Success => Status is >= 200 and < 300;
}

public abstract class ControllerBase
{
    protected static ServiceResult Ok(object? data) => new() { Status = 200, Data = data };

    protected static ServiceResult Response(int status, string message) =>
        new() { Status = status, Message = message };
}
