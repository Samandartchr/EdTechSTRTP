using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Infrastructure;
using Domains;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddScoped<CourseService>();
builder.Services.AddCors(o => o.AddPolicy("AllowFrontend", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var firebaseProjectId = builder.Configuration["Firebase:ProjectId"];
var serviceAccountPath = builder.Configuration["Firebase:ServiceAccountPath"];

FirebaseApp.Create(new AppOptions() {
    Credential = GoogleCredential.FromFile(serviceAccountPath),
    ProjectId = firebaseProjectId
});

var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();