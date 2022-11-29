using System;
using System.Security.Cryptography;
using System.Text;

namespace secure_webapp_api;

public static class PasswordHasher
{
  public static string Hash(string text, string salt)
  {
    if (string.IsNullOrEmpty(text))
      throw new ArgumentNullException();

    if (string.IsNullOrEmpty(salt))
      throw new ArgumentNullException();

    var sha = SHA1.Create();
    var saltedPassword = text + salt;

    var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(saltedPassword));

    var sb = new StringBuilder(hash.Length * 2);

    foreach (byte b in hash)
    {
      // can be "x2" if you want lowercase
      sb.Append(b.ToString("X2"));
    }

    return sb.ToString();
  }
}
