using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Configuration.AddEnvironmentVariables();

//Add our database
builder.Services.AddDbContext<EventDbContext>(options =>
{
    //options.UseModel(EventDbContextModel.Instance);
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Add the core services for handling actions from the bot
builder.Services
    .AddScoped<IGuildSettingsService, GuildSettingsService>()
    .AddScoped<IGuildEventService, GuildEventService>();

//Add NetCord services
builder.Services
    .AddDiscordShardedGateway(options =>
    {
        options.Intents = GatewayIntents.GuildVoiceStates;
    })
    .AddApplicationCommands<ApplicationCommandInteraction, ApplicationCommandContext>();

var app = builder.Build();

//Setup the pipeline
app
    .AddModules(typeof(Program).Assembly)
    .UseShardedGatewayHandlers();

await app.RunAsync();
