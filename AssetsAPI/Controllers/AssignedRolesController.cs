using AssetsAPI.Classes;
using AssetsAPI.Data;
using AssetsAPI.DTOs;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignedRolesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AssignedRolesController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<Boolean>> addAssignedRole(AssignedRoleDTO arole)
        {
            var userExists = await _context.Users.AnyAsync(u => u.id == arole.UserId);
            var roleExists = await _context.Roles.AnyAsync(r => r.id == arole.RoleId);

            if (!userExists || !roleExists) return BadRequest("UserId or RoleId does not exists");


            var roleFind = _context.AssignedRoles.FirstOrDefault(role => role.UserId == arole.UserId && role.RoleId == arole.RoleId);
            if (roleFind != null)
                return BadRequest("Role already exists");

            var _AssignRole = new AssignedRoles();

            _AssignRole.UserId = arole.UserId;
            _AssignRole.RoleId = arole.RoleId;
            var User = await _context.Users.FindAsync(arole.UserId);

            _AssignRole.User = User;
            _AssignRole.Role = await _context.Roles.FindAsync(arole.RoleId);



            _context.AssignedRoles.Add(_AssignRole);
            await _context.SaveChangesAsync();
            return Ok(true);
        }

        [HttpGet]
        public async Task<ActionResult<List<AssignedRolesReturn>>> getAllAssignedRoles()
        {
            var allAssignedRoles = await _context.AssignedRoles.Select(r => new AssignedRolesReturn
            {
                id = r.id,

                user = r.User.ConvertUserToUserReturn(),

                role = r.Role
            }).ToListAsync();

            return Ok(allAssignedRoles);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<AssignedRolesReturn>>> getAllAssignedRolesToUser(int userId)
        {
            var rolesToReturn = await _context.AssignedRoles
            .Where(r => r.User.id == userId)
            .Select(r => new AssignedRolesReturn
            {
                id = r.id,
                user = r.User.ConvertUserToUserReturn(),
                role = r.Role
            })
            .ToListAsync();


            if (!rolesToReturn.Any()) return BadRequest("No assigned roles for the given userid, or the userId is invalid");
            
           
            return Ok(rolesToReturn);
        }

      

        [HttpDelete("{roleid}/{userid}")]
        public async Task<ActionResult<Boolean>> deleteAssignedRoleToUser(int roleid, int userid)
        {
            var roleFind = _context.AssignedRoles.FirstOrDefault(asRole => asRole.RoleId == roleid && asRole.UserId == userid);
            if (roleFind == null) return BadRequest("It does not exist");

            _context.AssignedRoles.Remove(roleFind);
            await _context.SaveChangesAsync();

            return Ok(true);
        }

    }
}
