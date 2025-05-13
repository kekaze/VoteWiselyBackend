using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Pinecone;
using System.Text;
using VoteWiselyBackend.Extensions;
using VoteWiselyBackend.Factories.Implementations;
using VoteWiselyBackend.Factories.Interfaces;
using VoteWiselyBackend.Services;

var builder = WebApplication.CreateBuilder(args);

var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseSigningKey = builder.Configuration["Supabase:SigningKey"];
var pineconeApiKey = builder.Configuration["Pinecone:ApiKey"];
var pineconeHost = builder.Configuration["Pinecone:Host"];
var originUrl = builder.Configuration["OriginUrl"];
var hcaptchaSecretKey = builder.Configuration["HCaptcha:SecretKey"];

if (string.IsNullOrEmpty(supabaseUrl) ||
    string.IsNullOrEmpty(supabaseSigningKey) ||
    string.IsNullOrEmpty(pineconeApiKey) ||
    string.IsNullOrEmpty(pineconeHost) ||
    string.IsNullOrEmpty(originUrl) ||
    string.IsNullOrEmpty(hcaptchaSecretKey)
)
{
    throw new InvalidOperationException("An environment variable is not configured yet.");
};

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: "Policy-1",
        policy =>
        {
            policy.WithOrigins(originUrl)
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
builder.Services.AddSingleton<ISupabaseClientFactory, SupabaseClientFactory>();
await builder.Services.AddSupabaseClientAsync();

builder.Services.AddSingleton(sp =>
{
    PineconeClient pineconeClient = new PineconeClient(pineconeApiKey!);
    return new PineconeService(pineconeClient, pineconeHost!);
});

builder.Services.AddScoped<SupabaseService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DataTransformationService>();
builder.Services.AddScoped<HCaptchaService>(sp =>
    new HCaptchaService(
        sp.GetRequiredService<IHttpClientFactory>(),
        hcaptchaSecretKey
    )
);

builder.Services.AddHttpClient();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["AccessToken"];
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = $"{supabaseUrl}/auth/v1",
        ValidateAudience = true,
        ValidAudience = "authenticated",
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(supabaseSigningKey!)
        ),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Policy-1");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
