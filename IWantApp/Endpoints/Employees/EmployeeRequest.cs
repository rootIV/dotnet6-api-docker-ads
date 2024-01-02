namespace IWantApp.Endpoints.Employees;

public record EmployeeRequest(string email, string password, string name, string employeeCode);
