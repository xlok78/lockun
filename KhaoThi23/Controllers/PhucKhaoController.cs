using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KhaoThi23.Data;
using KhaoThi23.Models;
using KhaoThi23.DTO;

namespace KhaoThi23.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhucKhaoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PhucKhaoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PhucKhao
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhucKhao>>> GetPhucKhaos()
        {
            return await _context.PhucKhaos.Include(p => p.Employee).ToListAsync();
        }

        // GET: api/PhucKhao/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<List<PhucKhao>>> GetPhucKhao(string id)
        {
            var phucKhaos = await _context.PhucKhaos
                .Include(p => p.Employee)
                .Where(p => p.EmployeeId == id)
                .ToListAsync();

            if (phucKhaos == null || phucKhaos.Count == 0)
            {
                return NotFound();
            }

            return phucKhaos;
        }


        // PUT: api/PhucKhao/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhucKhao(int id, PhucKhao phucKhao)
        {
            if (id != phucKhao.PhucKhaoId)
            {
                return BadRequest();
            }

            // Kiểm tra sự tồn tại của bản ghi PhucKhao
            var existingPhucKhao = await _context.PhucKhaos.FindAsync(id);
            if (existingPhucKhao == null)
            {
                return NotFound();
            }

            // Kiểm tra sự tồn tại của Employee
            var employee = await _context.Employees.FindAsync(phucKhao.EmployeeId);
            if (employee == null)
            {
                return BadRequest("Invalid EmployeeId");
            }

            existingPhucKhao.Employee = employee;
            existingPhucKhao.MaLop = phucKhao.MaLop;
            existingPhucKhao.HocKy = phucKhao.HocKy;
            existingPhucKhao.NamHoc = phucKhao.NamHoc;
            existingPhucKhao.MaHocPhan = phucKhao.MaHocPhan;
            existingPhucKhao.TenHocPhan = phucKhao.TenHocPhan;
            existingPhucKhao.NgayGioThi = phucKhao.NgayGioThi;
            existingPhucKhao.PhongThi = phucKhao.PhongThi;
            existingPhucKhao.LanThi = phucKhao.LanThi;
            existingPhucKhao.LyDo = phucKhao.LyDo;
            existingPhucKhao.Status = phucKhao.Status;
            existingPhucKhao.UpdateAt = DateTime.Now;

            _context.Entry(existingPhucKhao).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhucKhaoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/PhucKhao
        [HttpPost]
        public async Task<ActionResult<PhucKhao>> PostPhucKhao(PhucKhaoDto phucKhaoDto)
        {
            // Kiểm tra sự tồn tại của Employee
            var employee = await _context.Employees.FindAsync(phucKhaoDto.EmployeeId);
            if (employee == null)
            {
                return BadRequest("Invalid EmployeeId");
            }

            var phucKhao = new PhucKhao
            {
                EmployeeId = phucKhaoDto.EmployeeId,
                MaLop = phucKhaoDto.MaLop,
                HocKy = phucKhaoDto.HocKy,
                NamHoc = phucKhaoDto.NamHoc,
                MaHocPhan = phucKhaoDto.MaHocPhan,
                TenHocPhan = phucKhaoDto.TenHocPhan,
                NgayGioThi = phucKhaoDto.NgayGioThi,
                PhongThi = phucKhaoDto.PhongThi,
                LanThi = phucKhaoDto.LanThi,
                LyDo = phucKhaoDto.LyDo,
                Status = phucKhaoDto.Status,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                Employee = employee
            };

            _context.PhucKhaos.Add(phucKhao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPhucKhao), new { id = phucKhao.PhucKhaoId }, phucKhao);
        }


        // DELETE: api/PhucKhao/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhucKhao(int id)
        {
            if (_context.PhucKhaos == null)
            {
                return NotFound();
            }
            var phucKhao = await _context.PhucKhaos.FindAsync(id);
            if (phucKhao == null)
            {
                return NotFound();
            }

            _context.PhucKhaos.Remove(phucKhao);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhucKhaoExists(int id)
        {
            return (_context.PhucKhaos?.Any(e => e.PhucKhaoId == id)).GetValueOrDefault();
        }
    }
}
