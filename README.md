### Asp.Net Core 6 Web Api | Dapper |Tsql

### GetCompanies

- repository:

```
     public async Task<IEnumerable<Company>> GetCompanies()
        {
            var query = "SELECT * FROM Companies";
            using (var connection = _context.CreateConnection())
            {
                var companies = await connection.QueryAsync<Company>(query);

                return companies.ToList();
            }
        }
```


-controller:

```

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _companyRepo.GetCompanies();
            return Ok(companies);
        }
```



### GetCompanyById

- repository:

```
    
        public async Task<Company> GetCompanyById(int id)
        {
            var query = "SELECT * FROM Companies WHERE Id=@Id";
            using (var connection = _context.CreateConnection())
            {
                var company = await connection.QuerySingleOrDefaultAsync<Company>(query, new { Id = id });

                return company;
            }

        }

```


-controller:

```

        [HttpGet("{id}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompany(int id)
        {
            var company = await _companyRepo.GetCompanyById(id);
            if (company is null)
                return NotFound();

            return Ok(company);
        }
```






### CreateCompany

- repository:

```
     public async Task<Company> CreateCompany(CompanyForCreationDto company)
        {
            var query = "INSERT INTO Companies (Name,Address,Country) VALUES (@Name,@Address,@Country)" +
                "SELECT CAST(SCOPE_IDENTITY() AS int)";

            var parameters = new DynamicParameters();
            parameters.Add("Name", company.Name, DbType.String);
            parameters.Add("Address", company.Address, DbType.String);
            parameters.Add("Country", company.Country, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parameters);

                var createdCompany = new Company
                {
                    Id = id,
                    Name = company.Name,
                    Address = company.Address,
                    Country = company.Country
                };
                return createdCompany;
            }

        }
```


-controller:

```
     [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            var createdCompany = await _companyRepo.CreateCompany(company);
            return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
        }

```




### UpdateCompany

- repository:

```
 public async Task UpdateCompany(int id, CompanyForUpdateDto company)
        {
            var query = "UPDATE Companies SET Name=@Name, Address=@Address, Country=@Country WHERE Id=@Id";

            var paramaters = new DynamicParameters();
            paramaters.Add("Id", id, DbType.Int32);
            paramaters.Add("Name", company.Name, DbType.String);
            paramaters.Add("Address", company.Address, DbType.String);
            paramaters.Add("Country", company.Country, DbType.String);


            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, paramaters);
            }

        }

```


-controller:

```
     [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] CompanyForUpdateDto company)
        {
            var dbCompany = await _companyRepo.GetCompanyById(id);
            if (dbCompany is null) throw new InvalidOperationException("Güncellenecek Şirket Bulunamadı");
            await _companyRepo.UpdateCompany(id, company);
            return NoContent();
        }

```




### DeleteCompany

- repository:

```
       public async Task DeleteCompany(int id)
        {
            var query = "DELETE FROM Companies WHERE Id=@Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
            }
        }

```


-controller:

```
 [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var dbCompany = await _companyRepo.GetCompanyById(id);
            if(dbCompany is null) return NotFound();

            await _companyRepo.DeleteCompany(id);

            return NoContent();
        }
```









### GetCompanyForEmployee

- repository:

```

        public async Task<Company> GetCompanyByEmployeeId(int id)
        {
            var procedureName = "ShowCompanyByEmployeeId";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32, ParameterDirection.Input);

            using (var connection = _context.CreateConnection())
            {
                var company = await connection.QueryFirstOrDefaultAsync<Company>(procedureName, parameters, commandType: CommandType.StoredProcedure);

                return company;
            }
        }

```


-controller:

```
     [HttpGet("ByEmployeeId/{id}")]
        public async Task<IActionResult> GetCompanyForEmployee(int id)
        {
            var company= await _companyRepo.GetCompanyByEmployeeId(id);
            if(company is null) return NotFound();

            return Ok(company);
        }
```










### GetMultipleResults

- repository:

```
 public async Task<Company> GetMultipleResults(int id)
        {
            var query = "SELECT * FROM Companies WHERE Id=@Id;" +
                "SELECT * FROM Employees WHERE CompanyId=@Id";

            using (var connection = _context.CreateConnection())
            using (var multi = await connection.QueryMultipleAsync(query, new { Id = id }))
            {
                var company = await multi.ReadSingleOrDefaultAsync<Company>();
                if (company is not null)
                    company.Employees = (await multi.ReadAsync<Employee>()).ToList();

                return company;
            }
        }

```


-controller:

```
   [HttpGet("{id}/MultipleResult")]
        public async Task<IActionResult> GetMultipleResults(int id)
        {
            var company = await _companyRepo.GetMultipleResults(id);
            if(company is null) return NotFound();

            return Ok(company);
        }
```




### GetMultipleResults

- repository:

```
public async Task<List<Company>> MultipleMapping()
        {
            var query = "Select * FROM Companies c JOIN Employees e ON c.Id= e.CompanyId";

            using (var connection = _context.CreateConnection())
            {
                var companyDict = new Dictionary<int,Company>();

                //<Company, Employee, Company>:Company, Employeegirsi türleri son tür olan Company dönüş türüdür
                var companies = await connection.QueryAsync<Company, Employee, Company>(
                    query, (Company, employee) =>
                    {
                        if(!companyDict.TryGetValue(Company.Id,out var currentCompany))
                        {
                            currentCompany = Company;
                            companyDict.Add(currentCompany.Id, currentCompany);
                        }
                        currentCompany.Employees.Add(employee);
                        return currentCompany;
                    });

                return companies.Distinct().ToList();
            }

        }

```


-controller:

```
 [HttpGet("MultipleMapping")]
        public async Task<IActionResult> GetMultipleMapping()
        {
            var companies = await _companyRepo.MultipleMapping();
            return Ok(companies);
        }
```
