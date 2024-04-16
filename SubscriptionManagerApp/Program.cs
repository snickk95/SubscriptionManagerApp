using Microsoft.EntityFrameworkCore;
using SubscriptionManagerApp.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.AppendTrailingSlash = true;
});

// Add services to the container.
//builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add CORS support:
builder.Services.AddCors(options => {
    options.AddPolicy("AllowTaskClients", policy => {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var connStr = builder.Configuration.GetConnectionString("SubscriptionManager");
builder.Services.AddDbContext<SubscriptionManagerContext>(options => options.UseSqlServer(connStr));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCors("AllowTaskClients");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//create a list of current users to set up the suthorization service
SubscriptionManagerContext context = new SubscriptionManagerContext();
List<User> user = context.Users
                .Select(u => new User()
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Subs = u.Subs
                }
            ).ToList();
//create a service for identity authorization
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
//addeach current user to the service by eamail
foreach (User u in user)
{
    using (var scope = scopeFactory.CreateScope())
    {
        await SubscriptionManagerContext.CreateUsers(scope.ServiceProvider,u.Email);
    }
}

app.Run();
