using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.Pagination;

namespace Server.Repository
{
    public class PhoneRepository : RepositoryBase<Phone>, IPhoneRepository
    {
        public PhoneRepository(PhoneshopIdentityContext repositoryContext)
            : base(repositoryContext) { }
        public Task<PagedList<Phone>> GetPhones(Page page)
        {
            return Task.FromResult(PagedList<Phone>.GetPagedList(FindAll().OrderBy(s=>s.Id),page.PageNumber,page.PageSize));
        }

        public async Task<PagedList<Phone>> SearchPhonesByName(string search, Page page)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                // Return an empty list or handle it according to your requirements
                return new PagedList<Phone>(new List<Phone>(), 0, page.PageNumber, page.PageSize);
            }

            var phones = FindAll().Where(p =>
                p.Name.ToLower().Contains(search.Trim().ToLower()) ||
                p.Description.ToLower().Contains(search.Trim().ToLower())
            );

            // Sử dụng ToListAsync nếu có sẵn
            var productList = await phones.OrderBy(p => p.Id).ToListAsync();

            return new PagedList<Phone>(productList, productList.Count, page.PageNumber, page.PageSize);
        }
        public async Task<PagedList<Phone>> AdvancedSearch(List<string> cpu, List<string> ramNames, List<string> brandName, decimal? minPrice, decimal? maxPrice, Page page)
        {
            var query = RepositoryContext.PhoneModels
                .Include(p => p.Ram)
                .Include(p => p.Brand)
                .Where(p =>
                    (cpu == null || cpu.Contains(p.CPU)) &&
                    (ramNames == null || ramNames.Contains(p.Ram.Name) && p.Ram.Status) &&
                    (brandName == null || brandName.Contains(p.Brand.Name.ToLower()) &&
                    (!minPrice.HasValue || p.Price >= minPrice) &&
                    (!maxPrice.HasValue || p.Price <= maxPrice)));

            var phoneModels = await query.OrderBy(p => p.Id).ToListAsync();

            // Convert PhoneModel objects to Phone objects
            var phones = phoneModels.Select(phoneModel => new Phone
            {
                Id = phoneModel.Id,
            }).ToList();

            return new PagedList<Phone>(phones, phones.Count, page.PageNumber, page.PageSize);
        }



    }
}
//    var query = RepositoryContext.PhoneModels
//        .Where(pm =>
//            (string.IsNullOrWhiteSpace(cpu) || pm.CPU.ToLower().Contains(cpu.Trim().ToLower())) &&
//            (ram <= 0 || pm.RamId == ram) &&    
//            (screenSize <= 0 || pm.SreenSize == screenSize) &&
//            (!minPrice.HasValue || !maxPrice.HasValue || (pm.Price >= minPrice.Value && pm.Price <= maxPrice.Value)))
//        .Join(
//            RepositoryContext.Phones,
//            phoneModel => phoneModel.PhoneId,
//            phone => phone.Id,
//            (phoneModel, phone) => phone
//        );