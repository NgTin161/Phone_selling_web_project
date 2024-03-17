using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Server.Data;
using Server.Models;
using Server.Pagination;
using Server.Repository;
using System.Xml.Linq;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhonesController : ControllerBase
    {
        private readonly PhoneshopIdentityContext _context;
        private IPhoneRepository _phonerepository;


        public PhonesController(PhoneshopIdentityContext context, IPhoneRepository phoneRepository)
        {
            _context = context;
            _phonerepository = phoneRepository;
        }
        private IEnumerable<Phone> PhonesModelFilter(string ramIds, string brandIds, string storageIds, string colorIds, int minPrice, int maxPrice)
        {
            IQueryable<PhoneModel> query = _context.PhoneModels;

            if (!string.IsNullOrEmpty(ramIds))
            {
                int[] ramIdsArray = ParseAndFilterIds(ramIds);

                if (ramIdsArray.Length == 1)
                {
              
                    int singleRamId = ramIdsArray[0];
                    query = query.Where(model => model.BrandId == singleRamId);
                }
                else if (ramIdsArray.Length > 1)
                {
                
                    query = query.Where(model => ramIdsArray.Contains(model.RamId));
                }
            }

            if (!string.IsNullOrEmpty(storageIds))
            {
                int[] storageIdsArray = ParseAndFilterIds(storageIds);
                if (storageIdsArray.Length == 1)
                {
            
                    int singlestorageId = storageIdsArray[0];
                    query = query.Where(model => model.StorageId == singlestorageId);
                }
                else if (storageIdsArray.Length > 1)
                {
                
                    query = query.Where(model => storageIdsArray.Contains(model.StorageId));
                }
            }

            if (!string.IsNullOrEmpty(brandIds))
            {
                int[] brandIdsArray = ParseAndFilterIds(brandIds);

                if (brandIdsArray.Length == 1)
                {
   
                    int singleBrandId = brandIdsArray[0];
                    query = query.Where(model => model.BrandId == singleBrandId);
                }
                else if (brandIdsArray.Length > 1)
                {
         
                    query = query.Where(model => brandIdsArray.Contains(model.BrandId));
                }
            }

            if (!string.IsNullOrEmpty(colorIds))
            {
                int[] colorIdsArray = ParseAndFilterIds(colorIds);
              
                if (colorIdsArray.Length == 1)
                {
                    // Trường hợp chỉ có một phần tử
                    int singleColorId = colorIdsArray[0];
                    query = query.Where(model => model.ColorId == singleColorId);
                }
                else if (colorIdsArray.Length > 1)
                {
                    // Trường hợp có nhiều phần tử
                    query = query.Where(model => colorIdsArray.Contains(model.BrandId));
                }
            }

            if (minPrice > 0)
            {
                query = query.Where(model => model.Price >= minPrice);
            }

            if (maxPrice > 0)
            {
                query = query.Where(model => model.Price <= maxPrice);
            }

            var filteredPhones = query
                .Select(model => model.Phone)
                .Where(phone => phone != null)
                .GroupBy(phone => phone.Id)
                .Select(group => group.First());

            return filteredPhones.ToList();
        }

        private int[] ParseAndFilterIds(string ids)
        {
            return ids.Split(',').Select(int.Parse).ToArray();
        }


        [HttpGet("filter")]
        public IActionResult FilterPhones([FromQuery] string  ramIds, [FromQuery] string  brandIds, [FromQuery]string storageIds, [FromQuery] string  colorIds, int minPrice, int maxPrice)
        {
            // Sử dụng phương thức PhonesModelFilter để lọc danh sách Phone
            var filteredPhones = PhonesModelFilter(ramIds, brandIds, storageIds, colorIds, minPrice, maxPrice).ToList();

            if (filteredPhones.Count == 0)
            {
              
                return NotFound("No phones found for the given criteria.");
            }

            return Ok(filteredPhones);
        }
        [HttpGet]
        [Route("demo")]
        public async Task<ActionResult<IEnumerable<Phone>>> GetPhones()
        {
            return await _context.Phones.Include(p => p.ProductType).ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Phone>>> GetProducts([FromQuery] Page page)
        {
            try
            {
                var productList = await _phonerepository.GetPhones(page);
                return Ok(productList);
            }
            catch (Exception ex)
            {
                // Log the exception or return an error response
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult<IEnumerable<Phone>>> GetPhoneByName([FromQuery] Page page, string search)
        {
            try
            {
                var productList = await _phonerepository.SearchPhonesByName(search, page);
                return Ok(productList);
            }
            catch
            {
                // Log the exception or return an error response
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Phone>> GetPhone(int id)
        {
            var phone = await _context.Phones.Include(p => p.ProductType)
                                            .FirstOrDefaultAsync(p => p.Id == id);
            if (phone == null)
            {
                return NotFound();
            }

            return phone;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhone(int id, Phone phone)
        {
            if (id != phone.Id)
            {
                return BadRequest();
            }

            _context.Entry(phone).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhoneExists(id))
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

        [HttpPost]
        public async Task<ActionResult<Phone>> PostPhone(Phone phone)
        {
            _context.Phones.Add(phone);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPhone", new { id = phone.Id }, phone);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhone(int id)
        {
            var phone = await _context.Phones.FindAsync(id);
            if (phone == null) { return NotFound(); }

            phone.Status = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhoneExists(int id)
        {
            return _context.Phones.Any(e => e.Id == id);
        }


        //[HttpGet]
        //[Route("Filter")]
        //public async Task<ActionResult<IEnumerable<Phone>>> AdvancedSearch (
        //     [FromQuery(Name = "cpu")]  List<string> cpu, [FromQuery(Name = "ram")] List<string> ram, [FromQuery(Name = "brandName")]  List<string> brandName, decimal? minPrice, decimal? maxPrice, Page page)
        //{
        //    try
        //    {
        //        var productList = await _phonerepository.AdvancedSearch(cpu, ram, brandName, minPrice, maxPrice, page);
        //        return Ok(productList);
        //    }
        //    catch
        //    {
        //        // Ghi log cho ngoại lệ hoặc trả về một phản hồi lỗi
        //        return NotFound();
        //    }
        //}

    }
}

