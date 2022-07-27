using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Belarus1.Data;
using Belarus1.Areas.Identity.Data;
using Belarus1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var connectionString = "Server=DESKTOP-NUA1Q5H;Database=AUTHDB;MultipleActiveResultSets=True;Trusted_Connection=True;";


builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlServer(connectionString)); 
builder.Services.AddDbContext<AUTHDBContext>(options =>
	options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<BelarusUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddRoles<IdentityRole>()
	.AddDefaultTokenProviders()
	.AddEntityFrameworkStores<IdentityContext>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();
app.MapRazorPages();
app.Run();
