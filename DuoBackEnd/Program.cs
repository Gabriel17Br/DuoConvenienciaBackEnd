using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Ativa o suporte para Controllers (para ler a pasta Controllers)
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 2. Configura o CORS (permite que seu HTML acesse essa API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Ativa a regra de acesso do CORS
app.UseCors("PermitirTudo");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// 3. Mapeia as rotas dos seus controllers automaticamente
app.MapControllers();

app.Run();

// 1. Recupera a string de conexão usando o nome exato que você definiu no JSON
var connectionString = builder.Configuration.GetConnectionString("AppDbConnectionString");

// 2. Detecta automaticamente a versão do seu servidor MySQL
var serverVersion = ServerVersion.AutoDetect(connectionString);

// 3. Registra o DbContext na API
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));