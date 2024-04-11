using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Hallodoc;
using HalloDoc.DataLayer.Data;

//using HalloDoc.DataLayer.Data;
using HalloDoc.DataLayer.ViewModels;
using HalloDoc.LogicLayer.Interface;
using HalloDoc.LogicLayer.Repository;
using HalloDoc.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

//to use session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options=>options.UseNpgsql(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));
//3tier
builder.Services.AddScoped<IPatient, Patient>();
builder.Services.AddScoped<IAdmin, HalloDoc.LogicLayer.Repository.Admin>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ILogin, Login>();
builder.Services.AddScoped<ICreateReq, CreateReq>();
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
//3tier

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=patientsite}/{id?}");

app.Run();
