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

using Microsoft.EntityFrameworkCore;
using PerfisdeInvestimento.API.Middleware;
using PerfisdeInvestimento.Application.Interfaces.IRepositories;
using PerfisdeInvestimento.Application.Interfaces.IServices;
using PerfisdeInvestimento.Application.Services;
using PerfisdeInvestimento.Infrastructure.Data;
using PerfisdeInvestimento.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar SQL LITE
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=../PerfisdeInvestimento.Infrastructure/perfisinvestimento.db"));


// Registrar os Repositórios
builder.Services.AddScoped<ISimulacaoRepository, SimulacaoRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IHistoricoInvestimentoRepository, HistoricoInvestimentoRepository>();

// Registrar os Services
builder.Services.AddScoped<ISimulacaoService, SimulacaoService>();
builder.Services.AddScoped<IRecomendacaoService, RecomendacaoService>();
builder.Services.AddScoped<IHistoricoInvestimentoService, HistoricoInvestimentoService>();


var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();
