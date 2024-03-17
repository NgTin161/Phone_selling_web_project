using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhoneModelsController : ControllerBase
    {
        private readonly PhoneshopIdentityContext _context;

        public PhoneModelsController(PhoneshopIdentityContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhoneModel>>> GetPhoneModels()
        {
            return await _context.PhoneModels.Include(p=>p.Phone)
                                              .Include(p=>p.Brand)
                                              .Include(p=>p.Color)
                                              .Include(p=>p.Storage)
                                              .Include(p=>p.Ram)
                                                .ToListAsync();     
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PhoneModel>> GetPhoneModel(int id)
        {
            var phoneModel = await _context.PhoneModels.Include(p => p.Phone)
                                              .Include(p => p.Brand)
                                              .Include(p => p.Color)
                                              .Include(p => p.Storage)
                                              .Include(p => p.Ram)
                                              .FirstOrDefaultAsync(p=>p.Id == id);
            if(phoneModel == null)
            {
                return NotFound();
            }

            return phoneModel;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhoneModel(int id, PhoneModel phoneModel)
        {
            if (id != phoneModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(phoneModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException )
            {
                if (!PhoneModelExists(id))
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
     
            

        //[HttpGet]
        //[Route("GetPhoneModelbyColorIdandStorageId")]
        //public async Task<ActionResult<IEnumerable<PhoneModel>>> GetPhoneModelbyColorIdandStorageId(int ColorId , int StorageId, int PhoneId )
        //{
        //    var phoneModel = await _context.PhoneModels.Include(p => p.Phone)
        //                                      .Include(p => p.Brand)
        //                                      .Include(p => p.Color)
        //                                      .Include(p => p.Storage)
        //                                      .Include(p => p.Ram)
        //                                      .Where(p => p.ColorId == ColorId && p.StorageId == StorageId && p.PhoneId == PhoneId).ToListAsync();

        //    return Ok(phoneModel);
        //}

        [HttpGet]
        [Route("GetPhoneModelsbyPhoneId")]
        public async Task<ActionResult<IEnumerable<object>>> LayMauVaBoNhoTheoPhoneId(int phoneId)
        {
            var phoneModels = await _context.PhoneModels
               .Include(pm=>pm.Phone)
              .Include(pm => pm.Color)
              .Include(pm => pm.Storage)
              .Include(pm=>pm.Ram)
              .Where(pm => pm.PhoneId == phoneId)
              .ToListAsync();

            if (phoneModels == null || !phoneModels.Any())
            {
                return NotFound();
            }

            // Lấy danh sách ColorId và StorageId từ danh sách phoneModels
            var colorIds = phoneModels.Select(pm => pm.ColorId).Distinct().ToList();
            var storageIds = phoneModels.Select(pm => pm.StorageId).Distinct().ToList();

         
            return Ok(phoneModels);
        }

        [HttpPost]
        public async Task<ActionResult<PhoneModel>> PostPhoneModel(PhoneModel phoneModel)
        {
            _context.PhoneModels.Add(phoneModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPhoneModel", new {id = phoneModel.Id}, phoneModel);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoneModel(int id)
        {
            var phoneModel = await _context.PhoneModels.FindAsync(id);
            if (phoneModel == null) { return NotFound(); }

            _context.PhoneModels.Remove(phoneModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhoneModelExists(int id)
        {
            return _context.PhoneModels.Any(x => x.Id == id);
        }
    }
}
