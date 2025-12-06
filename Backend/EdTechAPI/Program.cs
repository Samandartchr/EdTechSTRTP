using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Infrastructure;
using Domains;
using Services;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

var builder = WebApplication.CreateBuilder(args);

var firebaseProjectId = builder.Configuration["Firebase:ProjectId"];
var serviceAccountPath = builder.Configuration["Firebase:ServiceAccountPath"];

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile(serviceAccountPath),
        ProjectId = firebaseProjectId
    });
}

// Register FirebaseAuth as a singleton
builder.Services.AddSingleton(FirebaseAuth.DefaultInstance);
/*builder.Services.AddSingleton(provider =>
{
    return FirestoreDb.Create(firebaseProjectId);
});*/
var firestoreBuilder = new FirestoreClientBuilder
{
    Credential = GoogleCredential.FromFile(serviceAccountPath)
};
var firestoreClient = firestoreBuilder.Build();
builder.Services.AddSingleton(_ => FirestoreDb.Create(firebaseProjectId, firestoreClient));

builder.Services.AddScoped<CourseService>();

builder.Services.AddCors(o => o.AddPolicy("AllowFrontend", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://securetoken.google.com/" + firebaseProjectId;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://securetoken.google.com/" + firebaseProjectId,
            ValidateAudience = true,
            ValidAudience = firebaseProjectId,
            ValidateLifetime = true
        };
    });
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();