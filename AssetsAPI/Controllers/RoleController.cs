using AssetsAPI.Classes;
using AssetsAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AssetsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration; 

        public RoleController(DataContext context, IConfiguration configuration)
        {
            _context =context;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<Boolean>> addRole(string _roleName)
        {
            var roleFind = _context.Roles.FirstOrDefault(role => role.roleName == _roleName);
            if (roleFind != null)
                return BadRequest("Role already exists");

            Role role = new Role(); 
            role.roleName = _roleName;

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return Ok(true);
        }

        [HttpDelete]
        public async Task<ActionResult<Boolean>> deleteRole(string _roleName)
        {
            var roleFind = _context.Roles.FirstOrDefault(role => role.roleName == _roleName);
            if (roleFind == null)
                return BadRequest("Role does not exists");

            _context.Roles.Remove(roleFind);
            await _context.SaveChangesAsync();
            return Ok(true);
        }

        [HttpGet]
        public async Task<ActionResult<List<Role>>> getAllRoles(int userId)
        {
            var allRoles = await _context.Roles.Select(r => new Role
            {
                id = r.id,
                roleName = r.roleName,
            }).ToListAsync();

            var allAssignedRoles = await _context.AssignedRoles
                .Where(r => r.UserId == userId)
                .Select(r => new AssignedRolesReturn
                {
                    id = r.id,
                    user = r.User.ConvertUserToUserReturn(),
                    role = r.Role
                })
                .ToListAsync();

            foreach (var allAssignRole in allAssignedRoles)
            {
                if (allRoles.Contains(allAssignRole.role))
                {
                    allRoles.Remove(allAssignRole.role);
                }
            }

            return Ok(allRoles);
        }



    }
}
