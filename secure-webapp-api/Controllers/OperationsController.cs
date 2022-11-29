using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace secure_webapp_api;

[ApiController]
[Route("api/[controller]")]
public class OperationsController : ControllerBase
{
  private readonly AppDbContext dbContext;

  public OperationsController(AppDbContext dbContext)
  {
    this.dbContext = dbContext;
  }

  [HttpGet("users-table")]
  [Authorize]
  public ActionResult GetUsersTable()
  {
    var transformedList = dbContext.UserCreds.Select(x => new { x.Id, x.UserName, x.SaltedHashedPassword, Name = dbContext.Users.First(y => x.UserName == y.UserName).Name, LastName = dbContext.Users.First(y => x.UserName == y.UserName).LastName });
    var csvList = transformedList.Select(x => $"{x.Id};{x.UserName};{x.SaltedHashedPassword};{x.Name};{x.LastName}").ToList();
    csvList.Insert(0, "ID;USERNAME;SALTED-HASHED-PASSWORD;NAME;LASTNAME");

    return Ok(new { csv = string.Join(Environment.NewLine, csvList) });
  }

  [HttpPost("encrypt-file")]
  [Authorize]
  public ActionResult EncryptFile([FromForm] EncryptFileRequest request)
  {
    if (request is null) return BadRequest();
    if (request.file is null) return BadRequest();
    if (string.IsNullOrEmpty(request.Key)) return BadRequest();
    if (request.file.Length == 0) return BadRequest();

    string fileName = $"encrypted_{request.file.FileName}";
    using var reader = new StreamReader(request.file.OpenReadStream());
    string content = reader.ReadToEnd();

    var encryptedContent = AesEncrypter.EncryptString(request.Key, content);

    return Ok(new { encryptedContent, fileName });
  }

  public class EncryptFileRequest
  {
    public IFormFile? file { get; set; }
    public string? Key { get; set; }
  }
}
