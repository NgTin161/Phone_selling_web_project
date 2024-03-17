using Server.Models;
using Server.Pagination;
using Sever;

namespace Server.Repository
{
    public interface IPhoneRepository : IRepositoryBase<Phone>
    {
        Task<PagedList<Phone>> GetPhones(Page page);

        Task<PagedList<Phone>> SearchPhonesByName(string searchTerm, Page page);

        Task<PagedList<Phone>> AdvancedSearch(List<string> cpu, List<string> ramNames, List<string> brandName, decimal? minPrice, decimal? maxPrice, Page page);
    }


}

