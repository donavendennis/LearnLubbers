using DataAccess;
using DataAccess.DbInitializer;
using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Utility;
using System.Runtime.Loader;
using System.Reflection;
using Infrastructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
Console.WriteLine(connectionString);
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        connectionString
    )
);

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<DbInitializer>();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.ConfigureApplicationCookie(options =>
{
  options.LoginPath = $"/Identity/Account/Login";
  options.LogoutPath = $"/Identity/Account/Logout";
  options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddSingleton<IEmailSender, EmailSender>();

// Add distributed memory cache and session services
builder.Services.AddDistributedMemoryCache(); // For storing session in memory
builder.Services.AddSession(options =>
{
  options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
});

builder.Services.AddRazorPages();


// Load the unmanaged library
var context = new CustomAssemblyLoadContext();
context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "lib", "libwkhtmltox.dll"));

// Register the IConverter service
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<StudentDataService>();
builder.Services.AddScoped<InstructorDataService>();
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseMigrationsEndPoint();
}
else
{
  app.UseExceptionHandler("/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session middleware before authentication
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

SeedDatabase();

app.Run();

void SeedDatabase()
{
  using var scope = app.Services.CreateScope();
  var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
  dbInitializer.Initialize();
}

// Custom assembly load context to load unmanaged library
public class CustomAssemblyLoadContext : AssemblyLoadContext
{
  public IntPtr LoadUnmanagedLibrary(string absolutePath)
  {
    return LoadUnmanagedDll(absolutePath);
  }

  protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
  {
    return LoadUnmanagedDllFromPath(unmanagedDllName);
  }

  protected override Assembly Load(AssemblyName assemblyName)
  {
    throw new NotImplementedException();
  }
}
