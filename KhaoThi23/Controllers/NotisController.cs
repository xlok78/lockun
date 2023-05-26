using KhaoThi23.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KhaoThi23.Data;
using KhaoThi23.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace KhaoThi23.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Noti>>> GetAccounts()
        {
            if (_context.Notis == null)
            {
                return NotFound();
            }
            return await _context.Notis.ToListAsync();
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Noti>> Create([FromForm] NotiDto notiDto)
        {
            var noti = new Noti
            {
                Title = notiDto.Title,
                Content = notiDto.Content,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now
            };

            if (notiDto.Images != null)
            {
                // Lưu hình ảnh vào database
                using var memoryStream = new MemoryStream();
                await notiDto.Images.CopyToAsync(memoryStream);
                noti.Images = Convert.ToBase64String(memoryStream.ToArray());
            }

            _context.Notis.Add(noti);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = noti.NotiId }, noti);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Noti>> GetById(int id)
        {
            var noti = await _context.Notis.FindAsync(id);

            if (noti == null)
            {
                return NotFound();
            }

            if (noti.Images != null)
            {
                // Trả về hình ảnh dưới dạng base64 string
                noti.Images = "data:image/png;base64," + noti.Images;
            }

            return noti;
        }
    }
}
