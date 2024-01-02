using IWantApp.Domain.Users;
using IWantApp.Endpoints.Clients;

namespace IWantApp.Endpoints.Employees;

public class EmployeePost
{
    public static string Template => "/employees";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(EmployeeRequest employeeRequest, HttpContext http, UserCreator userCreator)
    {
        var authorizedUserMail = http.User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
        var userClaims = new List<Claim>
        {
            new Claim("EmployeeCode", employeeRequest.employeeCode),
            new Claim("Name", employeeRequest.name),
            new Claim("CreatedBy", authorizedUserMail)
        };

        (IdentityResult identity, string userId) result =
            await userCreator.Create(employeeRequest.email, employeeRequest.password, userClaims);

        if (!result.identity.Succeeded)
            return Results.ValidationProblem(result.identity.Errors.ConvertToProblemDetails());

        return Results.Created($"/employee/{result.userId}", result.userId);
    }
}

//var authorizedUserMail = http.User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
//var newUser = new IdentityUser { UserName = employeeRequest.email, Email = employeeRequest.email };
//var result = await userManager.CreateAsync(newUser, employeeRequest.password);

//if (!result.Succeeded)
//    return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());

//var userClaims = new List<Claim>
//{
//    new Claim("EmployeeCode", employeeRequest.employeeCode),
//    new Claim("Name", employeeRequest.name),
//    new Claim("CreatedBy", authorizedUserMail)
//};

//var claimResult = await userManager.AddClaimsAsync(newUser, userClaims);

//if (!claimResult.Succeeded)
//    return Results.BadRequest(claimResult.Errors.First());
