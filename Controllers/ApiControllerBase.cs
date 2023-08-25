using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Bearer")]
public abstract class ApiControllerBase : ControllerBase
{
    public int CurrentUserId
    {
        get
        {
            try
            {
                var userId = ((ClaimsIdentity)HttpContext.User.Identity).Claims.Where(c => c.Type == "UserId")
                    .Select(c => c.Value).SingleOrDefault();
                return Convert.ToInt32(userId);
            }
            catch (Exception ex)
            {
            }
            return 0;
        }
    }
}