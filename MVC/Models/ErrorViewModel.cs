namespace MVC.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
public class TokenResponse
{
    public string Token { get; set; }
    public string Role { get; set; }
}