using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURAÇÃO DE SERVIÇOS (Tudo antes do Build) ---

// Ativa o suporte para Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura o CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// Configuração do Banco de Dados (DbContext)
var connectionString = builder.Configuration.GetConnectionString("AppDbConnectionString");
var serverVersion = ServerVersion.AutoDetect(connectionString);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

// --- 2. CONSTRUÇÃO DA APLICAÇÃO ---
var app = builder.Build();

// --- 3. CONFIGURAÇÃO DO PIPELINE (Tudo após o Build) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("PermitirTudo");
app.UseAuthorization();

// Mapeia as rotas dos seus controllers
app.MapControllers();

app.Run();