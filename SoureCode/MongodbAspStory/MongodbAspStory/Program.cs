using MongodbAspStory.Models.BusinessModels;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<MongodbAspStoryDbContext>();
builder.Services.AddScoped<IRepositoryCategory, RepositoryCategory>();
builder.Services.AddScoped<IRepositoryAccount, RepositoryAccount>();
builder.Services.AddScoped<IRepositoryStory, RepositoryStory>();
builder.Services.AddScoped<IRepositoryChapter, RepositoryChapter>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(ops =>
{
	ops.IdleTimeout = TimeSpan.FromHours(1);
	ops.Cookie.Name = ".Bkap.Session";
	ops.Cookie.HttpOnly = true;
});
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.UseSession();
app.MapControllerRoute(
	name: "Admin",
	pattern: "{area:exists}/{controller=HomeAdmin}/{action=Index}/{id?}"
);

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
