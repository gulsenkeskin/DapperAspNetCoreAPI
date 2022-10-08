CREATE PROCEDURE [dbo].[ShowCompanyByEmployeeId]
@Id int
AS
SELECT c.Id, c.Name, c.Address, c.Country
FROM Companies c JOIN Employees e ON c.Id=e.CompanyId
WHERE e.Id=@Id