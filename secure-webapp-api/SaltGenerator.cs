using System;
using System.Security.Cryptography;

namespace secure_webapp_api;

public static class SaltGenerator
{
  public static string Generate()
  {
    RandomNumberGenerator rng = RandomNumberGenerator.Create();
    byte[] saltBytes = new byte[32];
    rng.GetBytes(saltBytes);

    return Convert.ToBase64String(saltBytes);
  }
}
