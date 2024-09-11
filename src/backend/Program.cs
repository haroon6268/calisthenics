using System.Text;
using CalisthenicsApi.Models;
using CalisthenicsApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<CalisthenicsDatabaseSettings>(
    builder.Configuration.GetSection("CalisthenicsDatabase")
);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();  // Adds console logging
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(cfg => {
    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters{
        
        //Validate who gave the token
        ValidateIssuer = false,
        //ValidIssuer = builder.Configuration["Jwt:Issuer"],

        //Validate the recepient of the token
        ValidateAudience = false,
        //ValidAudience = builder.Configuration["Jwt:Audience"],

        //Validate the signging key
        //ValidateIssuerSigningKey = true,

        //Here we setting the signingkey that the server will use to validate the signature of incoming tokens, and to also create the signatures of the keys
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
    };
    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context => {
            var token = context.Request.Cookies["Token"];
            context.Request.Headers.Append("Authorization", $"Bearer {token}") ;
            Console.WriteLine($"Token received: {context}");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
            {
                // Log the exception or failure reason
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                Console.WriteLine(context.Request.Headers["Authorization"]);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                // Log when a challenge is being issued
                Console.WriteLine($"Challenge issued: {context.Error}");
                return Task.CompletedTask;
            },
    };
});

//Singleton is one instance throughout the lifetime
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<WorkoutService>();

//A new instance gets created per request for scoped
//If 2 users use at the same time, you don't want data to get mixed up since they'd use the same instance
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
