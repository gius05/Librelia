using Librelia.Database;
using Librelia.Repositories;
using Librelia.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Librelia.Models;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(25667);
});

builder.Services.AddDataProtection()
    .SetApplicationName("Librelia");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var cultureInfo = new CultureInfo("en-US");
    options.DefaultRequestCulture = new RequestCulture(cultureInfo);
    options.SupportedCultures = new List<CultureInfo> { cultureInfo };
    options.SupportedUICultures = new List<CultureInfo> { cultureInfo };
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
    opt.TokenLifespan = TimeSpan.FromHours(2));
// Configurazione MongoDB 
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));

// Registra il Background Service
builder.Services.AddHostedService<CheckExpiredBooksService>();
builder.Services.AddHostedService<CleanUpResetPassword>();


// Configurazione EmailSettings 
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<EmailService>();           //Aggiunge il servizio per la newsletter


builder.Services.AddScoped<RecordGeneratorService>();   //Aggiunge il servizio per la generazione degli spreadsheet


// Inizializzazione Database
builder.Services.AddSingleton<MongoDBContext>();
builder.Services.AddSingleton<BookRepository>();
builder.Services.AddSingleton<ReservationRepository>();
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<ResetTokensRepository>();






// Configura l'autenticazione con i cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Pagina di login se l'utente non è autenticato
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";

        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Il cookie scade dopo 30 minuti
        options.SlidingExpiration = true; // Reset del timer se l'utente è attivo
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
/*if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseExceptionHandler("/Home/Error");

   app.UseHsts();
}*/
app.UseDeveloperExceptionPage();

// Gestione dell'errore 404
app.UseStatusCodePagesWithReExecute("/Home/NotFound");


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
