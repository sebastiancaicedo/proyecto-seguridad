using System;

namespace secure_webapp_api;

public class UserCreds
{
  public Guid Id { get; set; }
  public string UserName { get; set; } = string.Empty;
  public string Salt { get; set; } = string.Empty;
  public string SaltedHashedPassword { get; set; } = string.Empty;
}
