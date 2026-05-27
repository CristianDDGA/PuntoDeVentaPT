using FluentValidation;
using PuntoVenta.Infrastructure;
using PuntoVenta.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddValidatorsFromAssemblyContaining<PuntoVenta.Application.Validators.CreateSaleValidator>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<DataSeedingService>();

builder.Services.AddCors(corsOptions =>
    corsOptions.AddPolicy("BlazorPolicy", corsPolicy =>
        corsPolicy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("BlazorPolicy");
app.UseMiddleware<PuntoVenta.API.Middleware.ExceptionMiddleware>();
app.MapControllers();

app.Run();