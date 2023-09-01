using AutoMapper;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Identity.Controllers;



public class AccountController : ApiControllerBase
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IOptionsSnapshot<AppSettings> _settings;



    public AccountController(IMapper mapper,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptionsSnapshot<AppSettings> settings)
    {
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
        _settings = settings;
    }

    [HttpPost("RegisterUser")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(UserRegistrationModel userModel)
    {
        if (!ModelState.IsValid)
        {
            return Ok(userModel);
        }
        var user = _mapper.Map<ApplicationUser>(userModel);
        var result = await _userManager.CreateAsync(user, userModel.Password);
        var tk = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        if (!result.Succeeded)
        {

            return BadRequest(result.Errors);
        }
        return Ok(tk);


    }


    [HttpPost("AddUserRole")]
    public async Task<IActionResult> AddUserRole(UserRoleModel userRoleModel)
    {

        var user = await _userManager.FindByIdAsync(userRoleModel.UserId);
        var role = await _roleManager.FindByIdAsync(userRoleModel.RoleId);
        if (user != null && role != null)
        {
            await _userManager.AddToRoleAsync(user, role.Name);
            return Ok("Done");
        }

        return BadRequest("Error");

    }


    [HttpPost("GetRoleByUserId")]
    [HaveAccess("admin")]
    public async Task<IActionResult> GetRoleByUserId(string userId)
    {
        var dd = CurrentUserId;
        var user = await _userManager.FindByIdAsync(userId);
        var res = await _userManager.GetRolesAsync(user);
        return Ok(res);

    }


    [HttpPost("GetToken")]
    [AllowAnonymous]
    public async Task<IActionResult> GetToken(UserLogin usertoken)
    {
        var user = _userManager.FindByNameAsync(usertoken.UserName).Result;
        if (user != null)
        {
            var result = _userManager.CheckPasswordAsync
                (user, usertoken.Password).Result;

            if (!result)
            {
                return BadRequest(" User Not Found");
            }

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("FullName", $"{user.Name} {user.Family}")
            };

            var role = _userManager.GetRolesAsync(user).Result;
            if (role != null && role.Count > 0)
            {
                foreach (var item in role)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, item));
                }
            }
            var token = GetToken(authClaims);

            var tk = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(tk);
        }
        return BadRequest(" User Not Found");

    }
    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.SigningKey));

        var token = new JwtSecurityToken(
            issuer: _settings.Value.Issuer,
            audience: _settings.Value.Audience,
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }


    [AllowAnonymous]
    [HttpPost("CofirmEmail")]
    public async Task<IActionResult> CofirmEmail(ConfirmEmailModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            return BadRequest("User not Found");
        }
        var res = await _userManager.ConfirmEmailAsync(user, model.Token);
        if (!res.Succeeded)
        {
            return BadRequest("Invalid Token");
        }

        return Ok();

    }

    [AllowAnonymous]
    [HttpGet("GetProducts")]
    public async Task<IActionResult> GetProducts()
    {

        var d = GetListProducts();
        return Ok(d);
    }

    public class ProductModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public decimal price { get; set; }
    }

    private List<ProductModel> GetListProducts()
    {
        using var client = new HttpClient();
        var res = client.GetStringAsync("https://fakestoreapi.com/products").Result;
        var response = JsonSerializer.Deserialize<List<ProductModel>>(res);
        return response;

    }

}

