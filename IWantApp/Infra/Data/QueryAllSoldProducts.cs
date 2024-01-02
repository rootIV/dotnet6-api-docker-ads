namespace IWantApp.Infra.Data;

public class QueryAllSoldProducts
{
    private readonly IConfiguration configuration;

    public QueryAllSoldProducts(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<IEnumerable<ProductSoldReportResponse>> Execute()
    {
        var db = new SqlConnection(configuration["ConnectionStrings:IWantDb"]);
        var query = @"
            SELECT p.Id, p.Name, count (*) Amount 
            FROM IWantDb.dbo.Orders o INNER JOIN IWantDb.dbo.OrderProducts op
            ON o.Id = op.OrdersId
            INNER JOIN IWantDb.dbo.Products p 
            ON p.Id = op.ProductsId
            GROUP BY p.Id, p.Name
            ORDER BY Amount desc";

        return await db.QueryAsync<ProductSoldReportResponse>(query);
    } 
}