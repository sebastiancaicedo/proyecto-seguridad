namespace secure_webapp_api;

public class BaseResponse
{
  public bool Success { get; set; }
  public string Error { get; set; } = string.Empty;
}
