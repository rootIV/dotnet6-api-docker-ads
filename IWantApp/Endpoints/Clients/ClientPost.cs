using IWantApp.Domain.Users;

namespace IWantApp.Endpoints.Clients;

public class ClientPost
{
    public static string Template => "/clients";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(ClientRequest clientRequest, UserCreator userCreator)
    {
        var userClaims = new List<Claim>
        {
            new Claim("Cpf", clientRequest.cpf),
            new Claim("Name", clientRequest.name)
        };

        (IdentityResult identity, string userId) result =
            await userCreator.Create(clientRequest.email, clientRequest.password, userClaims);

        if (!result.identity.Succeeded)
            return Results.ValidationProblem(result.identity.Errors.ConvertToProblemDetails());

        return Results.Created($"/clients/{result.userId}", result.userId);
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
