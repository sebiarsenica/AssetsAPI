using AssetsAPI.Classes;
using AssetsAPI.Data;
using AssetsAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Globalization;
using Azure.Core;

namespace AssetsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public UserController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        [HttpPost("register")] 
        public async Task<ActionResult<List<User>>> registerUser(UserDto request)
        {
            CreatePasswordHash(request.password, out byte[] passwordHash, out byte[] passwordSalt);

            User user = new User(); 
            user.username = request.username;
            user.fullName = request.fullName;
            user.email = request.email;
            user.passwordHash = passwordHash;
            user.passwordSalt = passwordSalt;
            string dateString = DateTime.Now.ToString("dd-MM-yyyy");
            user.createDate = DateTime.ParseExact(dateString, "dd-MM-yyyy", CultureInfo.CurrentUICulture); //To fix in front end

            _context.Users.Add(user); 
            await _context.SaveChangesAsync();  

            return Ok(await _context.Users.ToListAsync());
        }

        [HttpPost("login")] 
        public async Task<ActionResult<string>> loginUser(UserDto request)
        {
           
            var userFind = _context.Users.FirstOrDefault(user => user.username == request.username);
            if (userFind == null)
             return BadRequest("User not found");
            
            if(!VerifyPasswordHash(request.password, userFind.passwordHash,userFind.passwordSalt))
            {
                return BadRequest("Wrong password");
            }

            string token = CreateToken(userFind);

            return Ok(JsonConvert.SerializeObject(token));

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<User>>> DeleteUser(int id)
        {
            var userFind = _context.Users.FirstOrDefault(user => user.id == id); 
            if(userFind == null) 
                return BadRequest("User not found"); 

            _context.Users.Remove(userFind);
            await _context.SaveChangesAsync();
            return Ok(await _context.Users.ToListAsync());
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.username)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

    }
}
