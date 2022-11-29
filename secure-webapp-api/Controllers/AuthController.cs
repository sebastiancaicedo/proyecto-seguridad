using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace secure_webapp_api;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

  private readonly IConfiguration configuration;
  private readonly AppDbContext dbContext;

  public AuthController(IConfiguration configuration, AppDbContext dbContext)
  {
    this.configuration = configuration;
    this.dbContext = dbContext;
  }

  [HttpPost]
  [AllowAnonymous]
  public ActionResult Auth([FromBody] AuthRequest request)
  {
    if (request is null) return BadRequest();
    if (string.IsNullOrEmpty(request.UserName)) return BadRequest();
    if (string.IsNullOrEmpty(request.Password)) return BadRequest();

    try
    {
      var loggedUser = AuthenticateUser(request.UserName, request.Password);

      if (loggedUser is null) return Unauthorized();

      string jwtSecretKey = configuration.GetValue<string>("JwtSecretKey", string.Empty);

      if (string.IsNullOrEmpty(jwtSecretKey)) throw new InvalidOperationException("jwt configuration error.");

      double expirationTime = configuration.GetValue<double>("tokenExpirationMinutes", 10);
      DateTime validUntil = DateTime.Now.AddMinutes(expirationTime);

      string token = GenerateAuthJwt(loggedUser, jwtSecretKey, expirationTime);

      if (string.IsNullOrEmpty(token)) throw new InvalidOperationException("Internal error");

      var response = new { token, validUntil, user = loggedUser };

      return Ok(response);
    }
    catch (Exception e)
    {
      var response = new AuthResponse(e.Message);
      return StatusCode(StatusCodes.Status500InternalServerError, response);
    }
  }

  private User? AuthenticateUser(string username, string password)
  {
    if (string.IsNullOrEmpty(username))
      throw new ArgumentException(nameof(username));

    if (string.IsNullOrEmpty(password))
      throw new ArgumentException(nameof(password));

    User? user = null;
    var credentials = dbContext.UserCreds.Where(x => x.UserName == username).FirstOrDefault();

    if (credentials is not null && CheckPasssword(password, credentials))
    {
      user = dbContext.Users.FirstOrDefault(x => x.UserName == credentials.UserName);
    }

    return user;
  }

  bool CheckPasssword(string password, UserCreds credentials)
  {
    string saltedhashedPassword = PasswordHasher.Hash(password, credentials.Salt);

    return saltedhashedPassword == credentials.SaltedHashedPassword;
  }

  private string GenerateAuthJwt(User user, string secretKey, double expirationTime)
  {
    if (string.IsNullOrEmpty(secretKey))
      throw new ArgumentException(nameof(secretKey));

    if (user.Id == Guid.Empty)
      throw new ArgumentException(nameof(user.Id));

    if (string.IsNullOrEmpty(user.Name))
      throw new ArgumentException(user.Name);

    Claim[] claims = new Claim[]
    {
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new Claim(ClaimTypes.Name, user.Name),
    };

    var key = Encoding.ASCII.GetBytes(secretKey);

    SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.UtcNow.AddMinutes(expirationTime),
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    JwtSecurityTokenHandler tokenHandler = new();
    var createdToken = tokenHandler.CreateToken(tokenDescriptor);

    return tokenHandler.WriteToken(createdToken);
  }


  [HttpPost("signup")]
  [AllowAnonymous]
  public ActionResult Register([FromBody] RegisterRequest registerUser)
  {
    if (registerUser is null) return BadRequest();
    if (string.IsNullOrEmpty(registerUser.Name) || string.IsNullOrEmpty(registerUser.LastName) || string.IsNullOrEmpty(registerUser.UserName) || string.IsNullOrEmpty(registerUser.Password))
      return BadRequest();

    //Validar datos
    string name = Regex.Replace(registerUser.Name, @"\p{C}+", "");
    string LastName = Regex.Replace(registerUser.LastName, @"\p{C}+", "");
    string username = Regex.Replace(registerUser.UserName, @"\p{C}+", "");

    string salt = SaltGenerator.Generate();
    var saltedhashedPassword = PasswordHasher.Hash(registerUser.Password, salt);

    UserCreds credentials = new()
    {
      UserName = username,
      Salt = salt,
      SaltedHashedPassword = saltedhashedPassword
    };

    dbContext.UserCreds.Add(credentials);

    User user = new()
    {
      Name = name,
      LastName = LastName,
      UserName = username,
    };

    var result = dbContext.Users.Add(user).Entity;

    dbContext.SaveChanges();

    return Ok(new { id = result.Id });
  }

}

public class RegisterRequest
{
  public string? Name { get; set; }
  public string? LastName { get; set; }
  public string? UserName { get; set; }
  public string? Password { get; set; }
}

public class AuthRequest
{
  public string? UserName { get; set; }
  public string? Password { get; set; }
}

public class AuthResponse : BaseResponse
{
  public string? Token { get; set; }
  public DateTime? ValidUntil { get; set; }

  public AuthResponse(string token, DateTime validUntil)
  {
    Success = true;
    Error = string.Empty;
    Token = token;
    ValidUntil = validUntil;
  }

  public AuthResponse(string error)
  {
    Error = error;
    Success = false;
    Token = null;
    ValidUntil = null;
  }
}
