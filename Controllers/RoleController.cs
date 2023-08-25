using AutoMapper;
using Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;


public class RoleController : ApiControllerBase
{
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;

  
    public RoleController(RoleManager<ApplicationRole> roleManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _mapper = mapper;
    }

    [HttpPost("RegisterRole")]
        public async Task<IActionResult> RegisterRole(RoleRegistrationModel roleModel)
        {
            var user = _mapper.Map<ApplicationRole>(roleModel);

            var result = await _roleManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("خطا");
            }

            return Ok();

        }

        [HttpPost("RoleList")]
        public async Task<IActionResult> RoleList()
        {
            var result = _roleManager.Roles.ToList();
            var res = _mapper.Map<List<RoleModel>>(result);
            return Ok(res);

        }

        [HttpPost("RoleDelete")]
        public async Task<IActionResult> RoleList(int id)
        {
            var result = await _roleManager.FindByIdAsync(id.ToString());
            if (result != null)
            {
                _roleManager.DeleteAsync(result);
                return Ok("deleted role");
            }

            return BadRequest("Role not found");

        }
    }

