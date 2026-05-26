using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PuntoVenta.Blazor;
using PuntoVenta.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// URL base de la API
builder.Services.AddScoped(serviceProvider => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5291/")
});

// Registrar servicios
builder.Services.AddScoped<CustomerApiService>();
builder.Services.AddScoped<ProductApiService>();
builder.Services.AddScoped<SaleApiService>();
builder.Services.AddScoped<DashboardApiService>();

await builder.Build().RunAsync();