using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Services.Consumers;
using Services.Data;
using Services.Helpers;
using Users.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -------------------
string jwtConfig;
string rabbitMQ;
string connectionString;
CloudinarySettings cloudinarySettings;

if (builder.Environment.IsProduction())
{
	jwtConfig = Environment.GetEnvironmentVariable("JWT");
	rabbitMQ = Environment.GetEnvironmentVariable("RABBIT_MQ");
	connectionString = Environment.GetEnvironmentVariable("SERVICES_CONNECTION_STRING");
	//connectionString = builder.Configuration.GetConnectionString("ServicesDB");
	cloudinarySettings = new CloudinarySettings
	{
		CloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME"),
		ApiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"),
		ApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
	};

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
	connectionString = builder.Configuration.GetConnectionString("ServicesDB");
	cloudinarySettings = builder.Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();

	// Database context - In memorys
	Console.WriteLine("--> Using InMem Db");
	builder.Services.AddDbContext<AppDbContext>(opt =>
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

// Automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Repo
builder.Services.AddScoped<IServiceRepo, ServiceRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IServiceCategoryRepo, ServiceCategoryRepo>();
builder.Services.AddScoped<IPhotoRepo, PhotoRepo>();

// Cloudinary
builder.Services.AddSingleton<CloudinarySettings>(cloudinarySettings);
//builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<IPhotoService, PhotoService>();

// Automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// MassTransit
builder.Services.AddMassTransit(config =>
{
	// register consumers
	config.AddConsumer<UserUpdatedConsumer>();
	config.AddConsumer<UserCreatedConsumer>();
	config.AddConsumer<UserDeletedConsumer>();

	config.UsingRabbitMq((ctx, cfg) =>
	{
		cfg.Host(rabbitMQ);

		// These are the consumers
		// creates exchange and queue with this name
		cfg.ReceiveEndpoint("Services_user-update-endpoint", c =>
		{
			// define the consumer class
			c.ConfigureConsumer<UserUpdatedConsumer>(ctx);
		});
		cfg.ReceiveEndpoint("Services_user-create-endpoint", c =>
		{
			// define the consumer class
			c.ConfigureConsumer<UserCreatedConsumer>(ctx);
		});
		cfg.ReceiveEndpoint("Services_user-delete-endpoint", c =>
		{
			// define the consumer class
			c.ConfigureConsumer<UserDeletedConsumer>(ctx);
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
// -------------------

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
