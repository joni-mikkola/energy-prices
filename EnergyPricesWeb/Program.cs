using EnergyPricesBL;
using EnergyPricesDB;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddLocalization(o => { o.ResourcesPath = "Resources"; });
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("de-DE"),
        new CultureInfo("fi-FI"),
        new CultureInfo("en-US")
    };

    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider());
});

builder.Services.AddSingleton(new SlackClient(builder.Configuration["SlackChannel"]));
builder.Services.AddDbContext<EnergyPricesDBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
//app.UseRequestLocalization();

var supportedCultures = new string[] { "de-DE", "fi-FI", "en-US" };
var requestLocalizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(requestLocalizationOptions.Value);


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(name: "default", pattern: "{culture=en-US}/{controller=Home}/{action=Index}");
});

if (app.Environment.IsDevelopment())
{
    app.UseSpa(spa =>
    {
        spa.UseProxyToSpaDevelopmentServer("https://localhost:3000");
    });
}
else
{
    app.MapFallbackToFile("index.html");
}

app.Run();
