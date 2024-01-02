namespace IWantApp.Endpoints.Products;

public class ProductGetShowcase
{
    public static string Template => "/products/showcase";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(ApplicationDbContext context, int page = 1, int rows = 10, string orderBy = "name")
    {
        if (rows > 10)
        {
            return Results.Problem(title: "Row invalid value, max 10!", statusCode: 400);
        }

        var queryBase = context.Products.AsNoTracking().Include(p => p.Category)
            .Where(p => p.HasStock && p.Category.Active);
        //.OrderBy(p => p.Name)

        if (orderBy == "name")
            queryBase = queryBase.OrderBy(p => p.Name);
        else if (orderBy == "price")
            queryBase = queryBase.OrderBy(p => p.Price);
        else
            return Results.Problem(title: "Order only by price or name", statusCode: 400);

        var queryFilter = queryBase.Skip((page - 1) * rows).Take(rows);

        var products = queryFilter.ToList();

        var results = products.Select(p => new ProductResponseId(p.Id ,p.Name, p.Category.Name, p.Description, p.Price, p.HasStock, p.Active));

        return Results.Ok(results);
    }
}
