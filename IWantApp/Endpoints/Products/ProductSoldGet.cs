namespace IWantApp.Endpoints.Products;

public class ProductSoldGet
{
    public static string Template => "/products/sold";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(QueryAllSoldProducts query)
    {
        var results = await query.Execute();

        return Results.Ok(results);
    }
}
