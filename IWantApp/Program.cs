using IWantApp.Domain.Users;
using IWantApp.Endpoints.Orders;

namespace IWantApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["ConnectionStrings:IWantDb"]);

        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 4;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddScoped<QueryAllUsersWithClaimName>();
        builder.Services.AddScoped<QueryAllSoldProducts>();
        builder.Services.AddScoped<UserCreator>();

        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => 
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateActor = false,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = false,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = builder.Configuration["JwtBearerTokenSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtBearerTokenSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtBearerTokenSettings:SecretKey"]))
            };
        });

        builder.Services.AddAuthorization(options => 
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
            options.AddPolicy("EmployeePolicy", p => p.RequireAuthenticatedUser());
            options.AddPolicy("CpfPolicy", p => p.RequireClaim("Cpf"));
            options.AddPolicy("EmployeePolicy", p => p.RequireClaim("EmployeeCode"));
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseAuthentication();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapMethods(CategoryPost.Template, CategoryPost.Methods, CategoryPost.Handle);
        app.MapMethods(CategoryGetAll.Template, CategoryGetAll.Methods, CategoryGetAll.Handle);
        app.MapMethods(CategoryPut.Template, CategoryPut.Methods, CategoryPut.Handle);
        app.MapMethods(EmployeeGetAll.Template, EmployeeGetAll.Methods, EmployeeGetAll.Handle);
        app.MapMethods(EmployeePost.Template, EmployeePost.Methods, EmployeePost.Handle);
        app.MapMethods(TokenPost.Template, TokenPost.Methods, TokenPost.Handle);
        app.MapMethods(ProductGetAll.Template, ProductGetAll.Methods, ProductGetAll.Handle);
        app.MapMethods(ProductPost.Template, ProductPost.Methods, ProductPost.Handle);
        app.MapMethods(ProductGetShowcase.Template, ProductGetShowcase.Methods, ProductGetShowcase.Handle);
        app.MapMethods(ProductSoldGet.Template, ProductSoldGet.Methods, ProductSoldGet.Handle);
        app.MapMethods(ClientGet.Template, ClientGet.Methods, ClientGet.Handle);
        app.MapMethods(ClientPost.Template, ClientPost.Methods, ClientPost.Handle);
        app.MapMethods(OrderPost.Template, OrderPost.Methods, OrderPost.Handle);
        app.MapMethods(OrderGet.Template, OrderGet.Methods, OrderGet.Handle);

        app.UseExceptionHandler("/error");
        app.Map("/error", (HttpContext http) =>
        {
            var error = http.Features?.Get<IExceptionHandlerFeature>()?.Error;

            if (error != null)
            {
                if (error is SqlException)
                {
                    return Results.Problem(title: "Database out", statusCode: 500);
                }
                else if (error is BadHttpRequestException)
                {
                    return Results.Problem(title: "Erro to convert data to other type. Review sent information", statusCode: 500);
                }
            }

            return Results.Problem(title: "An error ocurred", statusCode: 500);
        });

        app.UseAuthorization();

        app.Run();
    }
}