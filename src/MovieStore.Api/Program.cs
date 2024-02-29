using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using MovieStore.Database;
using Microsoft.Net.Http.Headers;
using MovieStore.Api.Data;
using MovieStore.Application.Interfaces;
using MovieStore.Application.Services;
using MovieStore.Hubs;
using MovieStore.Database.Cache;

var builder = WebApplication.CreateBuilder(args);

var jwtOptions = builder.Configuration
	.GetSection("Jwt")
    .Get<JwtOptions>();

builder.Services
    .AddDbContextPool<IdentityContext>(options => 
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteIdentityDatabase"));
    })
    .AddDbContextPool<MovieDbContext>(options => 
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteMoviesDatabase"));
    });

builder.Services
    .AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders();

builder.Services.AddStackExchangeRedisCache(redisOptions =>
{
    redisOptions.Configuration = builder.Configuration.GetConnectionString("Redis");
});

/************************************************
* Dependency Injection 
************************************************/
// Auth
builder.Services.AddSingleton(jwtOptions);
builder.Services.AddScoped<UserManager<IdentityUser<Guid>>>();
builder.Services.AddScoped<SignInManager<IdentityUser<Guid>>>();
builder.Services.AddScoped<RoleManager<IdentityRole<Guid>>>();

// Application
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IBranchService, BranchService>();

// Infrastructure DI only - API needs to DI into Application services
builder.Services.AddScoped<MovieDbContext>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();


builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAuthorization(/*options =>
{
    options.AddPolicy(IdentityData.AdminUserPolicyName, p =>
        p.RequireClaim(IdentityData.AdminUserClaimName, "true"));
    options.AddPolicy(IdentityData.ManagerUserPolicyName, p =>
        p.RequireClaim(IdentityData.ManagerUserClaimName, "true"));
}*/);
builder.Services
    .AddAuthentication(options => 
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options => 
    {
        //options.Authority = jwtOptions?.Issuer;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtOptions?.Issuer,
            ValidAudience = jwtOptions?.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions?.SigningKey ?? "NoKey")),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for our hub...
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken)
                    && path.StartsWithSegments("/hubs/orders"))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => 
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name =  HeaderNames.Authorization,
        Description = "Write 'Bearer Token'",
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme.ToLower(),
        BearerFormat = "JWT"
    });
    // options.AddSecurityRequirement(new OpenApiSecurityRequirement
    // {
    //     {
    //         new OpenApiSecurityScheme
    //         {
    //             In = ParameterLocation.Header,
    //             Name = JwtBearerDefaults.AuthenticationScheme,
    //             Reference = new OpenApiReference
    //             {
    //                 Type = ReferenceType.SecurityScheme,
    //                 Id = JwtBearerDefaults.AuthenticationScheme
    //             }
    //         },
    //         new List<string>()
    //     }
    // });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<OrderHub>("/hubs/orders");

app.Run();
