using DapperAspNetCoreAPI.Entities;

namespace DapperAspNetCoreAPI.Contracts
{
    public interface ICompanyRepository
    {
        public Task<IEnumerable<Company>> GetCompanies();

    }
}
