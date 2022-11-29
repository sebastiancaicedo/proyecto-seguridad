using System;

namespace secure_webapp_api;

public class User
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string UserName { get; set; } = string.Empty;
}
