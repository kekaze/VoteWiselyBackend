using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Pinecone;
using System.Text;
using VoteWiselyBackend.Extensions;
using VoteWiselyBackend.Factories.Implementations;
using VoteWiselyBackend.Factories.Interfaces;
using VoteWiselyBackend.Services;

var builder = WebApplication.CreateBuilder(args);

var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL");
var supabaseSigningKey = Environment.GetEnvironmentVariable("SUPABASE_SIGNING_KEY");
var pineconeApiKey = Environment.GetEnvironmentVariable("PINECONE_API_KEY");
var pineconeHost = Environment.GetEnvironmentVariable("PINECONE_INDEX_HOST");
var originUrl = Environment.GetEnvironmentVariable("ORIGIN_URL");

if (string.IsNullOrEmpty(supabaseUrl) ||
    string.IsNullOrEmpty(supabaseSigningKey) ||
    string.IsNullOrEmpty(pineconeApiKey) ||
    string.IsNullOrEmpty(pineconeHost) ||
    string.IsNullOrEmpty(originUrl)
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
    return new PineconeServices(pineconeClient, pineconeHost!);
});

builder.Services.AddScoped<SupabaseServices>();
builder.Services.AddScoped<AuthServices>();
builder.Services.AddScoped<DataTransformationServices>();
builder.Services.AddScoped<HCaptchaService>();

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
