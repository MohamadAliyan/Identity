using System.ComponentModel.DataAnnotations;

namespace Identity.Models;

public class UserRegistrationModel
{
    public string Name { get; set; }
    public string Family { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}
public class RoleRegistrationModel
{
    public string Name { get; set; }

}
public class RoleModel
{
    public string Name { get; set; }
    public string Id { get; set; }

}

public class UserRoleModel
{
    public string UserId { get; set; }
    public string RoleId { get; set; }

}
public class UserLogin
{
    public string UserName { get; set; }
    public string Password { get; set; }

}