//var builder = WebApplication.CreateBuilder(args);

//builder.AddServiceDefaults();

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//app.MapDefaultEndpoints();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using PerfisdeInvestimento.API.Middleware;
//using PerfisdeInvestimento.Application.Interfaces.IRepositories;
//using PerfisdeInvestimento.Application.Interfaces.IServices;
//using PerfisdeInvestimento.Application.Services;
//using PerfisdeInvestimento.Infrastructure.Data;
//using PerfisdeInvestimento.Infrastructure.Repositories;
//using PerfisdeInvestimento.Application.DTOs;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
////builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "Perfis de Investimento API",
//        Version = "v1",
//        Description = "API para simulação e recomendação de investimentos"
//    });

//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer",
//        BearerFormat = "JWT",
//        In = ParameterLocation.Header,
//        Description = "Digite 'Bearer' [espaço] e então seu token JWT.\nExemplo: \"Bearer abc123def456\""
//    });

//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            new string[] {}
//        }
//    });

//    options.MapType<ErroResponse>(() => new OpenApiSchema
//    {
//        Type = "object",
//        Properties = new Dictionary<string, OpenApiSchema>
//        {
//            ["tipo"] = new OpenApiSchema { Type = "string" },
//            ["titulo"] = new OpenApiSchema { Type = "string" },
//            ["mensagem"] = new OpenApiSchema { Type = "string" },
//            ["mensagemTecnica"] = new OpenApiSchema { Type = "string" },
//            ["status"] = new OpenApiSchema { Type = "integer" },
//            ["instancia"] = new OpenApiSchema { Type = "string" },
//            ["traceId"] = new OpenApiSchema { Type = "string" }
//        }
//    });
//});

//// Configurar SQL LITE
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlite("Data Source=../PerfisdeInvestimento.Infrastructure/perfisinvestimento.db"));


//// Registrar os Repositórios
//builder.Services.AddScoped<ISimulacaoRepository, SimulacaoRepository>();
//builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
//builder.Services.AddScoped<IHistoricoInvestimentoRepository, HistoricoInvestimentoRepository>();

//// Registrar os Services
//builder.Services.AddScoped<ISimulacaoService, SimulacaoService>();
//builder.Services.AddScoped<IRecomendacaoService, RecomendacaoService>();
//builder.Services.AddScoped<IHistoricoInvestimentoService, HistoricoInvestimentoService>();
//builder.Services.AddScoped<ITelemetriaService, TelemetriaService>();
//builder.Services.AddScoped<ITelemetriaRepository, TelemetriaRepository>();

//var jwtSettings = builder.Configuration.GetRequiredSection("Jwt");

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = jwtSettings["Issuer"],
//            ValidAudience = jwtSettings["Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)) 
//        };
//    });

//builder.Services.AddAuthorization();


//var app = builder.Build();

//// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
//app.UseMiddleware<UnauthorizedResponseMiddleware>();
//app.UseAuthentication();
//app.UseAuthorization();
//app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
//app.UseMiddleware<TelemetriaMiddleware>();
//app.UseHttpsRedirection();
//app.MapControllers();


//app.Run();

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PerfisdeInvestimento.API.Middleware;
using PerfisdeInvestimento.Application.Interfaces.IRepositories;
using PerfisdeInvestimento.Application.Interfaces.IServices;
using PerfisdeInvestimento.Application.Services;
using PerfisdeInvestimento.Infrastructure.Data;
using PerfisdeInvestimento.Infrastructure.Repositories;
using PerfisdeInvestimento.Application.DTOs;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger Configuration (mantenha igual)
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Perfis de Investimento API",
        Version = "v1",
        Description = "API para simulação e recomendação de investimentos"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite 'Bearer' [espaço] e então seu token JWT.\nExemplo: \"Bearer abc123def456\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    options.MapType<ErroResponse>(() => new OpenApiSchema
    {
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema>
        {
            ["tipo"] = new OpenApiSchema { Type = "string" },
            ["titulo"] = new OpenApiSchema { Type = "string" },
            ["mensagem"] = new OpenApiSchema { Type = "string" },
            ["mensagemTecnica"] = new OpenApiSchema { Type = "string" },
            ["status"] = new OpenApiSchema { Type = "integer" },
            ["instancia"] = new OpenApiSchema { Type = "string" },
            ["traceId"] = new OpenApiSchema { Type = "string" }
        }
    });
});

//AJUSTE CRÍTICO: Configurar SQLite para Container
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=perfisinvestimento.db"));

// Configurar SQLite - AJUSTE PARA USAR PASTA DATA
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Data/perfisinvestimento.db"));

// Registrar os Repositórios (mantenha igual)
builder.Services.AddScoped<ISimulacaoRepository, SimulacaoRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IHistoricoInvestimentoRepository, HistoricoInvestimentoRepository>();

// Registrar os Services (mantenha igual)
builder.Services.AddScoped<ISimulacaoService, SimulacaoService>();
builder.Services.AddScoped<IRecomendacaoService, RecomendacaoService>();
builder.Services.AddScoped<IHistoricoInvestimentoService, HistoricoInvestimentoService>();
builder.Services.AddScoped<ITelemetriaService, TelemetriaService>();
builder.Services.AddScoped<ITelemetriaRepository, TelemetriaRepository>();

// JWT Configuration (mantenha igual)
var jwtSettings = builder.Configuration.GetRequiredSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//ADICIONE: Aplicar Migrations Automaticamente no Container
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate(); // Isso aplica as migrations automaticamente
}

// Middlewares (mantenha igual)
app.UseMiddleware<UnauthorizedResponseMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<TelemetriaMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
