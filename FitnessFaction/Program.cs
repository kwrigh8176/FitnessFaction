using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.AspNetCore.Routing.Constraints;
using FitnessFaction.Database;
using FitnessFaction;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();

//adding our databases so that we can get their context from the controllers
builder.Services.AddScoped<AzureRDBMS_Connection, AzureRDBMS_Connection>();
builder.Services.AddScoped<MongoDatabaseConnection, MongoDatabaseConnection>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//basic settings
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

//a bunch of routing
app.MapControllerRoute(
   name: "Post",
   pattern: "Post/{id}",
   defaults: new { controller = "Post", action = "ViewPost" }
 );

app.MapControllerRoute(
   name: "createPost",
   pattern: "Post/CreatePost/{username}",
   defaults: new { controller = "Post", action = "CreatePost" }
 );

app.MapControllerRoute(
   name: "followAssistant",
   pattern: "Profile/Follow",
   defaults: new { controller = "ViewProfile", action = "Follow" }
 );

app.MapControllerRoute(
   name: "unfollowAssistant",
   pattern: "Profile/Unfollow",
   defaults: new { controller = "ViewProfile", action = "Unfollow" }
 );

app.MapControllerRoute(
   name: "Profile",
   pattern: "Profile/{username}",
   defaults: new { controller = "ViewProfile", action = "Profile" }
 );



app.MapControllerRoute(
   name: "switchAssistant",
   pattern: "Home/{action}",
   defaults: new { controller = "Home"}
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

