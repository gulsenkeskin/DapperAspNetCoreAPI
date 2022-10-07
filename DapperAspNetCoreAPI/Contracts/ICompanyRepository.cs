using DapperAspNetCoreAPI.Dto;
using DapperAspNetCoreAPI.Entities;

namespace DapperAspNetCoreAPI.Contracts
{
    public interface ICompanyRepository
    {
        public Task<IEnumerable<Company>> GetCompanies();
        public Task<Company> GetCompanyById(int id);
        public Task<Company> CreateCompany(CompanyForCreationDto company);
     

    }
}
