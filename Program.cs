using Pinecone;
using VoteWiselyBackend.Extensions;
using VoteWiselyBackend.Factories.Implementations;
using VoteWiselyBackend.Factories.Interfaces;
using VoteWiselyBackend.Services;

var builder = WebApplication.CreateBuilder(args);

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
            policy.WithOrigins(
                "http://localhost:8080"
            )
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

builder.Services.AddSingleton<ISupabaseClientFactory, SupabaseClientFactory>();
await builder.Services.AddSupabaseClientAsync();

builder.Services.AddSingleton<PineconeService>(sp =>
{
    string pineconeKey = Environment.GetEnvironmentVariable("PINECONE_API_KEY");
    string indexHost = Environment.GetEnvironmentVariable("PINECONE_INDEX_HOST");
    PineconeClient pineconeClient = new PineconeClient(pineconeKey);
    return new PineconeService(pineconeClient, indexHost);
});

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
