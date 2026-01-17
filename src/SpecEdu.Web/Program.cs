using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Infrastructure;
using SpecEdu.Web.Logging;
using SpecEdu.Web.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddRazorPages().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

var supportedCultures = new[] { "cs", "en", "de" }
    .Select(c => new CultureInfo(c))
    .ToList();

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("cs"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

localizationOptions.RequestCultureProviders = new IRequestCultureProvider[]
{
    new QueryStringRequestCultureProvider(),
    new CookieRequestCultureProvider(),
    new AcceptLanguageHeaderRequestCultureProvider()
};

app.UseRequestLocalization(localizationOptions);

Log.ApplicationStarting(logger, "0.0.8");
Log.ConfigurationLoaded(logger, "ConnectionStrings");
Log.ConfigurationLoaded(logger, "JwtSettings");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

Log.ApplicationStarted(logger);

app.Run();
