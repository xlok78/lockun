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
    public class NewsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NewsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/News
        [HttpGet]
        public async Task<ActionResult<IEnumerable<News>>> GetNews()
        {
          if (_context.News == null)
          {
              return NotFound();
          }
            return await _context.News.ToListAsync();
        }

        // GET: api/News/5
        [HttpGet("{id}")]
        public async Task<ActionResult<News>> GetNews(string id)
        {

            var news = await _context.News
                .Include(p => p.Employee)
                .Where(p => p.EmployeeId == id.ToString())
                .ToListAsync();

            if (news == null)
                {
                    return NotFound();
                }

            return Ok(news);
        }

        // PUT: api/News/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNews(int id, News news)
        {
            if (id != news.NewsId)
            {
                return BadRequest();
            }

            _context.Entry(news).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsExists(id))
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

        // POST: api/News
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<News>> PostNews(NewsDTO NewsDTO)
        {
            var employee = await _context.Employees.FindAsync(NewsDTO.EmployeeId);
            if (employee == null)
            {
                return BadRequest("Invalid EmployeeId");
            }

            var news = new News
            {
                NewsId = NewsDTO.NewsId,
                EmployeeId = employee.EmployeeId,
                Title = NewsDTO.Title,
                Status = NewsDTO.Status,
                Content1 = NewsDTO.Content1,
                Image1 = NewsDTO.Image1,
                ImageDesc1 = NewsDTO.ImageDesc1,
                Content2 = NewsDTO.Content2,
                Image2 = NewsDTO.Image2,
                ImageDesc2 = NewsDTO.ImageDesc2,
                Content3 = NewsDTO.Content3,
                Image3 = NewsDTO.Image3,
                ImageDesc3 = NewsDTO.ImageDesc3,
                Content4 = NewsDTO.Content4,
                Image4 = NewsDTO.Image4,
                ImageDesc4 = NewsDTO.ImageDesc4,
                Content5 = NewsDTO.Content5,
                Image5 = NewsDTO.Image1,
                ImageDesc5 = NewsDTO.ImageDesc5,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                
            };
            /*
            if (NewsDTO.Image1 != null)
            {
                // Lưu hình ảnh vào database
                using var memoryStream = new MemoryStream();
                await NewsDTO.Image1.CopyToAsync(memoryStream);
                News.Image1 = Convert.ToBase64String(memoryStream.ToArray());
            }
            */
            if (_context.News == null)
              {
                  return Problem("Entity set 'AppDbContext.News'  is null.");
              }
            _context.News.Add(news);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNews", new { id = news.NewsId }, news);
        }
        //đang bị lỗi nha vì bảng ko có cột status 
        // DELETE: api/News/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            if (_context.News == null)
            {
                return NotFound();
            }
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            _context.News.Remove(news);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NewsExists(int id)
        {
            return (_context.News?.Any(e => e.NewsId == id)).GetValueOrDefault();
        }
    }
}
