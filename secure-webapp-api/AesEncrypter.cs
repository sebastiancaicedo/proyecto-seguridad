using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace secure_webapp_api;

public static class AesEncrypter
{
  public static string EncryptString(string key, string decryptedString)
  {
    byte[] iv = new byte[16];
    byte[] array;
    string encryptedString;
    try
    {
      using (Aes aes = Aes.Create())
      {
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = iv;

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using (MemoryStream memoryStream = new MemoryStream())
        {
          using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
          {
            using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
            {
              streamWriter.Write(decryptedString);
            }

            array = memoryStream.ToArray();
          }
        }
      }

      encryptedString = Convert.ToBase64String(array);
    }
    catch (Exception)
    {
      encryptedString = string.Empty;
    }

    return encryptedString;
  }

}
