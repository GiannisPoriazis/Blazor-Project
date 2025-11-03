using AutoMapper;
using BlazorApp;
using BlazorApp.Components;
using BlazorApp.Interfaces;
using BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpClient();
builder.Services.AddScoped(sp =>
{
    var nav = sp.GetService<NavigationManager>();
    if (nav != null)
    {
        return new HttpClient { BaseAddress = new Uri(nav.BaseUri) };
    }

    var factory = sp.GetService<IHttpClientFactory>();
    var client = factory?.CreateClient() ?? new HttpClient();
    client.BaseAddress ??= new Uri(builder.Configuration["BaseAddress"] ?? "https://localhost/");
    return client;
});

builder.Services.AddDbContext<BlazorAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddMaps(typeof(BlazorApp.Mapping.CustomerProfile).Assembly);
});

IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mappingConfig);
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorApp.Client._Imports).Assembly);


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapControllers();
app.Run();

public partial class Program { }