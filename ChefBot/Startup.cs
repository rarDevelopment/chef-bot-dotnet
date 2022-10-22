global using Discord;
global using Discord.Interactions;
global using Discord.WebSocket;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Logging;
using System.Reflection;
using ChefBot;
using ChefBot.Models;
using DiscordDotNetUtilities;
using DiscordDotNetUtilities.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


var builder = new HostBuilder();

var f = builder.ConfigureAppConfiguration(options
    => options.AddJsonFile("appsettings.json")
        .AddUserSecrets(Assembly.GetEntryAssembly(), true)
        .AddEnvironmentVariables());

var loggerConfig = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File($"logs/log-{DateTime.Now:dd.MM.yy_HH.mm}.log")
    .CreateLogger();

builder.ConfigureServices((host, services) =>
{
    services.AddLogging(options => options.AddSerilog(loggerConfig, dispose: true));
    services.AddSingleton(new DiscordSocketClient(
        new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.All,
            FormatUsersInBidirectionalUnicode = false,
            AlwaysDownloadUsers = true,
            LogGatewayIntentWarnings = false
        }));

    var isDevelopment = host.HostingEnvironment.IsDevelopment();
    var discordSettings = new DiscordSettings
    {
        BotToken = isDevelopment
            ? host.Configuration["Discord:BotToken"]
            : Environment.GetEnvironmentVariable("BOT_TOKEN")
    };

    services.AddSingleton(discordSettings);
    services.AddScoped<IDiscordFormatter, DiscordFormatter>();
    services.AddScoped<IFoodRepository, FoodRepository>();
    services.AddScoped<IFoodMessageGenerator, FoodMessageGenerator>();

    services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));

    services.AddSingleton<InteractionHandler>();

    services.AddHostedService<DiscordBot>();
});

var app = builder.Build();

await app.RunAsync();