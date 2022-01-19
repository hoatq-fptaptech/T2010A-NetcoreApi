using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using T2010A_WEBAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
namespace T2010A_WEBAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly T2010AContext _context;
        private IConfiguration _configuration; // để có thể lấy đc các config trong file appsetting.json
        public LoginController(IConfiguration config,T2010AContext context)
        {
            _context = context;
            _configuration = config;
        }

        [HttpPost]
        public async Task<IActionResult> Login(Admin adminData)
        {
            //1. Kiểm tra tài khoản admin gửi lên có trong db hay ko
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == adminData.Username && a.Password == adminData.Password);
            //2. Xây dựng 1 jwt trả về
            if(admin!= null)
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub,_configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat,DateTime.Now.ToString()),
                    new Claim("Id",admin.Id.ToString()),
                    new Claim("Username",admin.Username)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],_configuration["Jwt:Audience"],claims,
                    expires:DateTime.Now.AddDays(1),signingCredentials:sign);
                // trả token về cho client
                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            return BadRequest();
        }
    }
}
