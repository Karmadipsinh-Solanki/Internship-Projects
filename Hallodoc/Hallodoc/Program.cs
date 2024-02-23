using Hallodoc.Data;
using HalloDoc.DataLayer.ViewModels;
using HalloDoc.LogicLayer.Patient_Interface.LoginControllerInterface;
using HalloDoc.LogicLayer.Patient_Interface.LoginInterface;
using HalloDoc.LogicLayer.Patient_Interface.PatientDashboardInterface;
using HalloDoc.LogicLayer.Patient_Interface.PatientRequest;
using HalloDoc.LogicLayer.Patient_Repository.LoginRepository;
using HalloDoc.LogicLayer.Patient_Repository.PatientDashboardRepository;
using HalloDoc.LogicLayer.Patient_Repository.PatientRequest;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options=>options.UseNpgsql(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));
//3tier
builder.Services.AddScoped<IPatientLogin, PatientLogin>();
builder.Services.AddScoped<IResetPasswordFromEmail, ResetPasswordFromEmail>();
builder.Services.AddScoped<IForgotPwd, ForgotPwd>();
builder.Services.AddScoped<IPatientDashboard, PatientDashboard>();
builder.Services.AddScoped<IViewDoc, ViewDoc>();
builder.Services.AddScoped<IMeModalSubmit, MeModalSubmit>();
builder.Services.AddScoped<IMeModal, MeModal>();
builder.Services.AddScoped<IRelativeModalSubmit, RelativeModalSubmit>();
builder.Services.AddScoped<IProfile, Profile>();
builder.Services.AddScoped<ICreatePatientRequest, CreatePatientRequest>();
builder.Services.AddScoped<ICreatePatientRequest, CreatePatientRequest>();
builder.Services.AddScoped<ICreatePatientRequest, CreatePatientRequest>();
builder.Services.AddScoped<ICreatePatientRequest, CreatePatientRequest>();
builder.Services.AddScoped<IPatientCheck, PatientCheck>();
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
