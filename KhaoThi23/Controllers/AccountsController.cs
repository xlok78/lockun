using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KhaoThi23.Data;
using KhaoThi23.Models;
using System.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NuGet.Protocol.Plugins;
using Microsoft.AspNetCore.Authorization;
using KhaoThi23.DTO;

namespace KhaoThi23.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountsController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Accounts
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
          if (_context.Accounts == null)
          {
              return NotFound();
          }
            return await _context.Accounts.ToListAsync();
        }

        // POST: api/Accounts/login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var account = await _context.Accounts
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AccountEmail == request.AccountEmail);

            if (account == null || !BCrypt.Net.BCrypt.Verify(request.AccountPassword, account.AccountPassword))
            {
                var error = new
                {
                    message = "Email hoặc mật khẩu không đúng"
                };
                return Ok(error);
            }

            // Lấy EmployeeId của nhân viên được liên kết với tài khoản đang đăng nhập
            var employeeId = account.Employee?.EmployeeId;

            // Tạo claims chứa thông tin của người dùng (EmployeeId và AccountRole)
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Role, account.AccountRole),
        new Claim("EmployeeId", employeeId)
    };

            // Tạo mã JWT token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(creds);
            var payload = new JwtPayload(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                null,
                DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])));
            var token = new JwtSecurityToken(header, payload);

            // Tạo đối tượng chứa message và token
            var response = new
            {
                message = "Đăng nhập thành công",
                token = new JwtSecurityTokenHandler().WriteToken(token)
            };

            return Ok(response);
        }
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAccount([FromBody] AccountCreateRequest request)
        {
            // Kiểm tra xem email đã tồn tại trong database chưa
            var existingAccount = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountEmail == request.AccountEmail);
            if (existingAccount != null)
            {
                return Ok("Email đã tồn tại");
            }

            // Mã hóa mật khẩu bằng BCrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.AccountPassword);

            // Tạo mới tài khoản
            var newAccount = new Account
            {
                AccountId = "AC-" + Guid.NewGuid().ToString(),
                AccountEmail = request.AccountEmail,
                AccountPassword = hashedPassword,
                AccountRole = request.AccountRole,
                CreateAt = DateTime.Now.ToString(),
                UpdateAt = DateTime.Now.ToString()
            };

            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();

            // Tạo mới nhân viên
            var newEmployee = new Employee
            {
                EmployeeId = "EL-" + Guid.NewGuid().ToString(),
                EmployeeGender = request.EmployeeGender,
                EmployeeName = request.EmployeeName,
                EmployeeMSSV = request.EmployeeMSSV,
                UpdateAt = DateTime.Now.ToString(),
                CreateAt = DateTime.Now.ToString(),
                AccountId = newAccount.AccountId
            };

            _context.Employees.Add(newEmployee);
            await _context.SaveChangesAsync();

            return Ok("Create Thành công");
        }

        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/Accounts/changepass/{employeeId}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("changepass/{employeeId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ChangePassword(string employeeId, ChangePasswordRequest request)
        {
            var account = await _context.Accounts
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Employee.EmployeeId == employeeId);

            if (account == null || !BCrypt.Net.BCrypt.Verify(request.OldPassword, account.AccountPassword))
            {
                var error = new
                {
                    message = "Mật khẩu cũ không đúng"
                };
                return BadRequest(error);
            }

            account.AccountPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(account.AccountId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var response = new
            {
                message = "Đổi mật khẩu thành công"
            };

            return Ok(response);
        }

        private bool AccountExists(string id)
        {
            return (_context.Accounts?.Any(e => e.AccountId == id)).GetValueOrDefault();
        }
    }
}
