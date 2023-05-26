using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KhaoThi23.Data;
using KhaoThi23.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using KhaoThi23.DTO;

namespace KhaoThi23.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Employees
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<IEnumerable<EmployeeAccountDto>>> GetEmployees()
        {
            var employeeAccounts = await _context.Employees
                .Join(_context.Accounts,
                    e => e.AccountId,
                    a => a.AccountId,
                    (e, a) => new EmployeeAccountDto
                    {
                        EmployeeId = e.EmployeeId,
                        EmployeeGender = e.EmployeeGender,
                        EmployeeMSSV = e.EmployeeMSSV,
                        EmployeeName = e.EmployeeName,
                        AccountEmail = a.AccountEmail,
                        AccountRole = a.AccountRole
                    })
                .ToListAsync();

            if (employeeAccounts == null || employeeAccounts.Count == 0)
            {
                return NotFound();
            }

            return employeeAccounts;
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeAccountDto>> GetEmployee(string id)
        {
            var employeeAccount = await _context.Employees
                .Join(_context.Accounts,
                    e => e.AccountId,
                    a => a.AccountId,
                    (e, a) => new EmployeeAccountDto
                    {
                        EmployeeId = e.EmployeeId,
                        EmployeeGender = e.EmployeeGender,
                        EmployeeMSSV = e.EmployeeMSSV,
                        EmployeeName = e.EmployeeName,
                        AccountEmail = a.AccountEmail,
                        AccountRole = a.AccountRole
                    })
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employeeAccount == null)
            {
                return NotFound();
            }

            return Ok(employeeAccount);
        }

        // POST: api/Employees/update
        [HttpPost("update")]
        public async Task<IActionResult> UpdateEmployee(EmployeeAccountDto employeeAccountDto)
        {
            // Tìm nhân viên theo ID
            var existingEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeAccountDto.EmployeeId);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            // Tìm tài khoản theo ID
            var existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == existingEmployee.AccountId);
            if (existingAccount == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin nhân viên
            existingEmployee.EmployeeName = employeeAccountDto.EmployeeName;
            existingEmployee.EmployeeGender = employeeAccountDto.EmployeeGender;
            existingEmployee.EmployeeMSSV = employeeAccountDto.EmployeeMSSV;
            // Cập nhật các thuộc tính khác tương tự

            // Cập nhật thông tin tài khoản
            existingAccount.AccountEmail = employeeAccountDto.AccountEmail;
            existingAccount.AccountRole = employeeAccountDto.AccountRole;
            // Cập nhật các thuộc tính khác tương tự

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.Entry(existingEmployee).State = EntityState.Modified;
                _context.Entry(existingAccount).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                transaction.Commit();

                return Ok("Cập nhật thông tin thành công");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        // DELETE: api/Employees/EL-1
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                // Xóa Employee
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id);
                if (employee == null)
                {
                    return NotFound();
                }

                _context.Employees.Remove(employee);

                // Xóa Account
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == employee.AccountId);
                if (account == null)
                {
                    return NotFound();
                }

                _context.Accounts.Remove(account);

                await _context.SaveChangesAsync();
                transaction.Commit();

                return NoContent();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private bool EmployeeExists(string id)
        {
            return (_context.Employees?.Any(e => e.EmployeeId == id)).GetValueOrDefault();
        }
    }
}
