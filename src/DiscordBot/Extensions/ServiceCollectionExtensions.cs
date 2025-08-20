using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Models;
using DiscordBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Extensions;

/// <summary>
/// Extension methods for registering Discord bot services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Discord bot configuration, client, and related services into the DI container.
    /// </summary>
    public static IServiceCollection AddDiscordBot(this IServiceCollection services, IConfiguration configuration)
    {
        // Load bot configuration
        var botConfig = configuration.GetSection("Bot").Get<BotConfig>() ?? new BotConfig();
        services.AddSingleton(botConfig);

        // Configure and register Discord client
        var socketConfig = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged |
                                GatewayIntents.MessageContent |
                                GatewayIntents.Guilds |
                             GatewayIntents.GuildMessages |
                             GatewayIntents.GuildMembers,
            LogLevel = LogSeverity.Info
        };
        services.AddSingleton(socketConfig);
        services.AddSingleton<DiscordSocketClient>();

        // Core bot services
        services.AddSingleton<CommandRegistryService>();
        services.AddSingleton<CommandHandlerService>();
        services.AddSingleton<DiscordBotService>();

        return services;
    }
}
