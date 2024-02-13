using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using MovieStore.Data;
using MovieStore.Database;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

var jwtOptions = builder.Configuration
	.GetSection("Jwt")
    .Get<JwtOptions>();

builder.Services.AddDbContextPool<IdentityContext>(options => 
{
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteIdentityDatabase"));
});

builder.Services
    .AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddSingleton(jwtOptions);
builder.Services.AddScoped<UserManager<IdentityUser<Guid>>>();
builder.Services.AddScoped<SignInManager<IdentityUser<Guid>>>();


builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services
.AddAuthentication(options => 
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => 
{
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

app.Run();
