namespace IWantApp.Infra.Data;

public class QueryAllUsersWithClaimName
{
    private readonly IConfiguration configuration;

    public QueryAllUsersWithClaimName(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<IEnumerable<employeeResponse>> Execute(int page, int rows)
    {
        var db = new SqlConnection(configuration["ConnectionStrings:IWantDb"]);
        var query = @"
            SELECT Email, ClaimValue AS Name
            FROM IWantDb.dbo.AspNetUsers u INNER JOIN IWantDb.dbo.AspNetUserClaims c
            ON u.id = c.UserId AND ClaimType = 'Name'
            ORDER BY name
            OFFSET (@page -1) * @rows ROWS FETCH NEXT @rows ROWS ONLY";

        return await db.QueryAsync<employeeResponse>(query, new { page, rows });
    }
    public bool PageRowsValidation(int? page, int? rows)
    {
        if (!page.HasValue || !rows.HasValue)
            return false;

        const int minPage = 1;
        const int maxPage = 10;
        const int minRows = 1;
        const int maxRows = 10;

        if (page < minPage || page > maxPage || rows < minRows || rows > maxRows)
            return false;

        return true;
    }
}