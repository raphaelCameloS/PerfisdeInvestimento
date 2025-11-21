using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PerfisdeInvestimento.API.Middleware;
using PerfisdeInvestimento.Application.DTOs;
using PerfisdeInvestimento.Application.Interfaces.IRepositories;
using PerfisdeInvestimento.Application.Interfaces.IServices;
using PerfisdeInvestimento.Application.Services;
using PerfisdeInvestimento.Infrastructure.Data;
using PerfisdeInvestimento.Infrastructure.Repositories;
using System.Reflection;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Perfis de Investimento API",
        Version = "v1",
        Description = "API para simulacao e recomendacao de investimentos de acordo com perfil desejado. Para gerar token nessa versao utilize pessoaUsuaria:admin e Senha: admin123. Alem disso, um clienteId valido para fins de testes: 123"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite 'Bearer' [espaco] e entao seu token JWT.\nExemplo: \"Bearer abc123def456\""
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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Data/perfisinvestimento.db"));

builder.Services.AddScoped<ISimulacaoRepository, SimulacaoRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IHistoricoInvestimentoRepository, HistoricoInvestimentoRepository>();


builder.Services.AddScoped<ISimulacaoService, SimulacaoService>();
builder.Services.AddScoped<IRecomendacaoService, RecomendacaoService>();
builder.Services.AddScoped<IHistoricoInvestimentoService, HistoricoInvestimentoService>();
builder.Services.AddScoped<ITelemetriaService, TelemetriaService>();
builder.Services.AddScoped<ITelemetriaRepository, TelemetriaRepository>();

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


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}


app.UseMiddleware<UnauthorizedResponseMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<TelemetriaMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();