using EventSignup.Data;
using EventSignup.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database Context
builder.Services.AddDbContext<EventDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var azureConnectionString = builder.Configuration.GetConnectionString("AzureBlobStorage");
if (!string.IsNullOrEmpty(azureConnectionString) && azureConnectionString != "Your_Azure_Blob_Storage_Connection_String_Here")
{
    builder.Services.AddScoped<IBlobService, AzureBlobService>();
}
else
{
// Required for generate absolute URLs for locally stored images.
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IBlobService, LocalBlobService>();
}

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<EventDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
