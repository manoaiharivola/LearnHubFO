using LearnHubFO.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddScoped<UtilisateursService>();
builder.Services.AddScoped<CoursService>();
builder.Services.AddScoped<CoursUtilisateurService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Utilisateurs/Login";
            options.AccessDeniedPath = "/Utilisateurs/AccessDenied";
        });

builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var user = context.User;
    if (!user.Identity.IsAuthenticated && !context.Request.Path.StartsWithSegments("/Utilisateurs"))
    {
        context.Response.Redirect("/Utilisateurs/Login");
    }
    else if (user.Identity.IsAuthenticated && context.Request.Path == "/")
    {
        context.Response.Redirect("/Home/Index");
    }
    else
    {
        await next();
    }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
