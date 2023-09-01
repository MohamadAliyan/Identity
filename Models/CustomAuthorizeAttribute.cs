using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Identity.Models;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
public class HaveAccessAttribute : Attribute, IAuthorizationFilter
{
    private string _Role;
    private RoleManager<ApplicationRole> _roleManager;
    private UserManager<ApplicationUser> _userManager;

    public HaveAccessAttribute(string role)

    {
        _Role = role;
    } 
    
    public  void OnAuthorization(AuthorizationFilterContext filterContext)
    {
        var user = filterContext.HttpContext.User;
        if (!user.Identity.IsAuthenticated)
        {
            filterContext.Result = new UnauthorizedResult();
            return;
        }
      
        _userManager = filterContext.HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();
        var userId =  user.Claims.Where(c => c.Type == "UserId")
            .Select(c => c.Value).SingleOrDefault();
        var userf =  _userManager.FindByIdAsync(userId).Result;
        var roles =  _userManager.GetRolesAsync(userf).Result;

        var isAuthorized = roles.ToList()[0].ToLower() == _Role;
        if (!isAuthorized)
        {
            filterContext.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Unauthorized);
            return;
        }

        // base.OnAuthorization(filterContext);
    }


    
}