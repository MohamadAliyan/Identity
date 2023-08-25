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
        if (!result.Succeeded)
        {
          
            return BadRequest(result.Errors);
        }
        return Ok();
        

    }


    [HttpPost("AddUserRole")]
    public async Task<IActionResult> AddUserRole(UserRoleModel userRoleModel)
    {
        var user =await _userManager.FindByIdAsync(userRoleModel.UserId);
        var role = await _roleManager.FindByIdAsync(userRoleModel.RoleId);
        if (user != null && role != null)
        {
            await _userManager.AddToRoleAsync(user, role.Name);
            return Ok("Done");
        }

        return BadRequest("Error");

    }


    [HttpPost("GetRoleByUserId")]
    public async Task<IActionResult> GetRoleByUserId(string userId)
    {
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


    //[HttpPost("GetToken")]

    //public async Task<IActionResult> Logout()
    //{
    //    await _signInManager.SignOutAsync();
    //    return Ok("Exit");
    //}

}

