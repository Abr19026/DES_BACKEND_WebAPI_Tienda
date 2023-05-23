using WebAPI_Tienda;

var builder = WebApplication.CreateBuilder(args);

// Configura servicios de la api
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

// Configura Middleware
var app = builder.Build();
startup.Configure(app, app.Environment);


app.Run();
