using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Negotiate; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();


builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorPages();
//builder.Services.AddIdentity<IdentityUser, IdentityRole>

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

app.UseSession();

app.MapControllerRoute(
   name: "SwitchFeedType",
   pattern: "Home/SwitchFeed",
   defaults: new { controller = "Home", action = "SwitchFeed" }
 );

app.MapControllerRoute(
   name: "Home",
   pattern: "Home/{username}",
   defaults: new { controller = "Home", action = "HomeFeed"}
 );



app.MapControllerRoute(
              name: "Error",
              pattern: "Error/{action}",
              defaults: new { controller = "Error", action = "ForbiddenError" }
          );

app.MapControllerRoute(
            name: "Default",
            pattern: "{controller}/{action}",
            defaults: new { controller = "Login", action = "Login"},
            new { controller = "Login" }
        );



app.Run();

