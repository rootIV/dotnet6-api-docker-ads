using IWantApp.Domain.Users;
using static System.Net.WebRequestMethods;

namespace IWantApp.Endpoints.Clients;

public class ClientGet
{
    public static string Template => "/clients";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(HttpContext http)
    {
        var user = http.User;
        var result = new
        {
            Id = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value,
            Email = user.Claims.First(c => c.Type == "Name").Value,
            Cpf = user.Claims.First(c => c.Type == "Cpf").Value,
        };

        return Results.Ok(result);
    }
}

//var newUser = new IdentityUser { UserName = clientRequest.email, Email = clientRequest.email };
//var result = await userManager.CreateAsync(newUser, clientRequest.password);

//if (!result.Succeeded)
//    return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());

//var userClaims = new List<Claim>
//{
//    new Claim("Cpf", clientRequest.cpf),
//    new Claim("Name", clientRequest.name)
//};

//var claimResult = await userManager.AddClaimsAsync(newUser, userClaims);

//if (!claimResult.Succeeded)
//    return Results.BadRequest(claimResult.Errors.First());
