using Microsoft.EntityFrameworkCore;
using Users.Data;
using MassTransit;
using Users.Consumers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// ----------------
string jwtConfig;
string rabbitMQ;
string connectionString;

if (builder.Environment.IsProduction())
{
	jwtConfig = Environment.GetEnvironmentVariable("JWT");
	rabbitMQ = Environment.GetEnvironmentVariable("RABBIT_MQ");
	connectionString = Environment.GetEnvironmentVariable("USERS_CONNECTION_STRING");
	//connectionString = builder.Configuration.GetConnectionString("UsersDB");

	// Database context - SQL server
	Console.WriteLine("--> Using SqlServer Db");
	builder.Services.AddDbContext<AppDbContext>(opt =>
		// specify database type and name
		opt.UseSqlServer(connectionString)
	);
}
else
{
	jwtConfig = builder.Configuration["JwtConfig:Secret"];
	rabbitMQ = $"amqp://guest:guest@{builder.Configuration["RabbitMQHost"]}:{builder.Configuration["RabbitMQPort"]}";
	connectionString = builder.Configuration.GetConnectionString("UsersDB");

	// Database context - In memory
	Console.WriteLine("--> Using InMem Db");
	builder.Services.AddDbContext<AppDbContext>(opt =>
		// specify database type and name
		opt.UseInMemoryDatabase("InMem")
	);
}

// Authentication
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	// in case first one fails
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
// JWT token configuration
.AddJwtBearer(jwt =>
{
	// how it should be encoded
	var key = Encoding.ASCII.GetBytes(jwtConfig);

	// jwt token settings
	jwt.SaveToken = true;
	jwt.TokenValidationParameters = new TokenValidationParameters
	{
		// validate third part of the token using the secret and check if it was configured and encrypted by us
		ValidateIssuerSigningKey = true,
		// define signing key (responsible for encrypting)
		IssuerSigningKey = new SymmetricSecurityKey(key),
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateLifetime = true,
		// should be true in production
		RequireExpirationTime = false,
	};
});

// User Repo
builder.Services.AddScoped<IUserRepo, UserRepo>();

// Automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// MassTransit
builder.Services.AddMassTransit(config =>
{
	// register consumer
	config.AddConsumer<UserConsumer>();

	config.UsingRabbitMq((ctx, cfg) =>
	{
		Console.WriteLine(rabbitMQ);
		cfg.Host(rabbitMQ);

		// This is the consumer
		// creates exchange and queue with this name
		cfg.ReceiveEndpoint("Users_user-endpoint", c =>
		{
			// define the consumer class
			c.ConfigureConsumer<UserConsumer>(ctx);
		});
	});
});

// Cors
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
	options.AddPolicy(name: MyAllowSpecificOrigins,
		policy =>
		{
			policy.WithOrigins(
				"http://localhost:4200",
				"http://localhost:80",
				"http://20.79.236.59"
			)
			.AllowAnyMethod()
			.AllowAnyHeader();
		});
});

// Health checks
builder.Services.AddHealthChecks();
// ----------------

builder.Services.AddControllers();
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

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/healthz");

app.MapControllers();

// Prep data
PrepDb.PrepPopulation(app);

app.Run();
