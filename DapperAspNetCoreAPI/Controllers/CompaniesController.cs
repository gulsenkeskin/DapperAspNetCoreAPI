using DapperAspNetCoreAPI.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DapperAspNetCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepo;
        public CompaniesController(ICompanyRepository companyRepo)
        {
            _companyRepo = companyRepo;
        }



        [HttpGet]   
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _companyRepo.GetCompanies();
            return Ok(companies);
        }

        [HttpGet("{id}", Name ="CompanyById")]
        public async Task<IActionResult> GetCompany(int id)
        {
            var company=await _companyRepo.GetCompanyById(id);
            if(company is null) 
                return NotFound();

            return Ok(company); 
        }

    }
}
