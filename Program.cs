// --- 1. Importações (Usings) ---
using InventarioAPI.Data;
using InventarioAPI.Models; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

// --- 2. Criação do Construtor da Aplicação ---
var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration; 

// --- 3. Configuração dos Serviços (Injeção de Dependência) ---

// Configura o EF Core para usar um banco de dados em memória.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("InventarioDB");
});

// Configura a autenticação JWT Bearer.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validações padrão
            ValidateIssuer = true, 
            ValidateAudience = true, 
            ValidateLifetime = true, 
            ValidateIssuerSigningKey = true, 
            
            // Valores vindos do appsettings.json
            ValidIssuer = Configuration["Jwt:Issuer"], 
            ValidAudience = Configuration["Jwt:Audience"], 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]!)) 
        };
    });

// Habilita o serviço de Autorização.
builder.Services.AddAuthorization();

// Adiciona serviços do Swagger/OpenAPI para documentação e testes da API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Adiciona a definição de segurança (JWT Bearer)
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Insira seu token JWT desta forma: Bearer {seu_token}"
    });

    // Adiciona o requisito de segurança (o "cadeado") aos endpoints
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// --- 4. Construção da Aplicação ---
var app = builder.Build();

// --- 5. Configuração do Pipeline de Requisições (Middlewares) ---

// Habilita o Swagger apenas em ambiente de desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Gera a interface gráfica do Swagger
}

// IMPORTANTE: A ordem dos middlewares é crucial.
app.UseAuthentication(); // 1º: Tenta autenticar (verificar quem é)
app.UseAuthorization();  // 2º: Verifica se a pessoa autenticada pode acessar


// --- 6. Definição dos Endpoints da API ---

/// <summary>
/// Endpoint raiz de verificação (Health Check).
/// </summary>
app.MapGet("/", () => "API de Inventário no ar!")
   .WithTags("Health Check");


// Grupo de Endpoints de Autenticação
var authGroup = app.MapGroup("/auth")
    .WithTags("Autenticação");

/// <summary>
/// Registra um novo usuário no sistema.
/// </summary>
authGroup.MapPost("/register", async (LoginRequest request, AppDbContext db) =>
{
    var userExists = await db.Usuarios.FirstOrDefaultAsync(u => u.Username == request.Username);
    if (userExists != null)
    {
        return Results.BadRequest("Usuário já existe.");
    }

    string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

    var newUser = new Usuario
    {
        Username = request.Username,
        PasswordHash = passwordHash
    };

    db.Usuarios.Add(newUser);
    await db.SaveChangesAsync();

    return Results.Created($"/auth/users/{newUser.Id}", "Usuário criado com sucesso.");
});

/// <summary>
/// Autentica um usuário e retorna um token JWT.
/// </summary>
authGroup.MapPost("/login", async (LoginRequest request, AppDbContext db, IConfiguration config) =>
{
    var user = await db.Usuarios.FirstOrDefaultAsync(u => u.Username == request.Username);

    if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
    {
        // Resposta genérica por segurança (não dizer se foi o usuário ou a senha que errou)
        return Results.BadRequest("Usuário ou senha inválidos.");
    }

    var token = GerarTokenJwt(user, config); 
    return Results.Ok(new { token = token });
});


// Grupo de Endpoints do Inventário (Protegidos)
var inventarioGroup = app.MapGroup("/api/ativos")
    .WithTags("Inventário de Ativos")
    .RequireAuthorization(); // <-- Protege todos os endpoints deste grupo

/// <summary>
/// Lista todos os ativos do inventário. Requer autenticação.
/// </summary>
inventarioGroup.MapGet("/", async (AppDbContext db) =>
{
    var ativos = await db.Ativos.ToListAsync();
    return Results.Ok(ativos);
});

/// <summary>
/// Busca um ativo específico pelo ID. Requer autenticação.
/// </summary>
inventarioGroup.MapGet("/{id}", async (int id, AppDbContext db) =>
{
    var ativo = await db.Ativos.FindAsync(id);
    if (ativo is null)
    {
        return Results.NotFound("Ativo não encontrado.");
    }
    return Results.Ok(ativo);
});

/// <summary>
/// Cadastra um novo ativo no inventário. Requer autenticação.
/// </summary>
inventarioGroup.MapPost("/", async (Ativo ativo, AppDbContext db) =>
{
    db.Ativos.Add(ativo);
    await db.SaveChangesAsync();
    
    return Results.Created($"/api/ativos/{ativo.Id}", ativo);
});

/// <summary>
/// Atualiza um ativo existente. Requer autenticação.
/// </summary>
inventarioGroup.MapPut("/{id}", async (int id, Ativo inputAtivo, AppDbContext db) =>
{
    var ativo = await db.Ativos.FindAsync(id);

    if (ativo is null) return Results.NotFound("Ativo não encontrado.");

    ativo.NomeDoHost = inputAtivo.NomeDoHost;
    ativo.SistemaOperacional = inputAtivo.SistemaOperacional;
    ativo.Status = inputAtivo.Status;

    await db.SaveChangesAsync();
    return Results.Ok(ativo);
});

/// <summary>
/// Deleta um ativo pelo ID. Requer autenticação.
/// </summary>
inventarioGroup.MapDelete("/{id}", async (int id, AppDbContext db) =>
{
    var ativo = await db.Ativos.FindAsync(id);

    if (ativo is null)
    {
        return Results.NotFound("Ativo não encontrado.");
    }

    db.Ativos.Remove(ativo);
    await db.SaveChangesAsync();
    
    return Results.NoContent();
});


// --- 7. Funções Auxiliares ---

/// <summary>
/// Gera um token JWT para um usuário autenticado.
/// </summary>
/// <param name="user">O objeto do usuário vindo do banco.</param>
/// <param name="config">A configuração da aplicação (para ler a chave secreta).</param>
/// <returns>Uma string com o token JWT.</returns>
string GerarTokenJwt(Usuario user, IConfiguration config)
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
    
    // *** CORREÇÃO AQUI ***
    // O algoritmo correto é HmacSha256
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    // Claims são as "informações" que vão dentro do token
    var claims = new[] 
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username), // "Subject" (quem é)
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // "JWT ID" (ID único do token)
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // ID do usuário no banco
    }; 

    var token = new JwtSecurityToken(
        issuer: config["Jwt:Issuer"],
        audience: config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddHours(8), // Duração do token
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
}


// --- 8. Execução da Aplicação ---
app.Run();