using Discord;
using Discord.WebSocket;
using DiscordBot.Commands;
using DiscordBot.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Services;

/// <summary>
/// Handles incoming messages, parses commands, 
/// checks permissions, and executes registered commands.
/// </summary>
public class CommandHandlerService
{
    private readonly CommandRegistryService _commandRegistry;
    private readonly IServiceProvider _serviceProvider;
    private readonly BotConfig _config;
    private readonly ILogger<CommandHandlerService> _logger;

    public CommandHandlerService(
        CommandRegistryService commandRegistry,
        IServiceProvider serviceProvider,
        BotConfig config,
        ILogger<CommandHandlerService> logger)
    {
        _commandRegistry = commandRegistry;
        _serviceProvider = serviceProvider;
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Handles a Discord message and determines if it's a valid command.
    /// </summary>
    public async Task HandleMessageAsync(SocketMessage message)
    {
        if (message.Author.IsBot) return;

        if (message is not SocketUserMessage userMessage) return;

        var client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
        if (client?.CurrentUser == null) return;

        // Ensure the bot is mentioned before processing
        if (!userMessage.MentionedUsers.Any(u => u.Id == client.CurrentUser.Id))
            return;

        // Extract command text after mention
        var content = userMessage.Content;
        var mentionPrefix = $"<@{client.CurrentUser.Id}>";
        var mentionPrefixNick = $"<@!{client.CurrentUser.Id}>";

        string commandText;
        if (content.StartsWith(mentionPrefix))
            commandText = content[mentionPrefix.Length..].Trim();
        else if (content.StartsWith(mentionPrefixNick))
            commandText = content[mentionPrefixNick.Length..].Trim();
        else
            return;

        if (string.IsNullOrWhiteSpace(commandText))
            return;

        var parts = commandText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        var commandName = parts[0].ToLower();
        var args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

        await ExecuteCommandAsync(message, commandName, args);
    }

    /// <summary>
    /// Executes a registered command if found and checks permissions.
    /// </summary>
    private async Task ExecuteCommandAsync(SocketMessage message, string commandName, string[] args)
    {
        var commandInfo = _commandRegistry.GetCommand(commandName);
        if (commandInfo == null)
        {
            await SendErrorMessage(message, $"❌ Command `{commandName}` not found.");
            return;
        }

        // Check user permissions for this command
        var permissionCheck = await CheckPermissionsAsync(message, commandInfo);
        if (!permissionCheck.HasPermission)
        {
            await SendErrorMessage(message, permissionCheck.ErrorMessage!);
            return;
        }

        try
        {
            // Create command instance with DI and execute
            var command = (BaseCommand)ActivatorUtilities.CreateInstance(_serviceProvider, commandInfo.CommandType);
            await command.ExecuteAsync(message, args);

            _logger.LogInformation("Executed command {Command} by {User}", commandName, message.Author.Username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing command {Command}", commandName);
            await SendErrorMessage(message, "❌ An error occurred while executing the command.");
        }
    }

    /// <summary>
    /// Checks whether the user has the required guild and channel permissions.
    /// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private async Task<(bool HasPermission, string? ErrorMessage)> CheckPermissionsAsync(
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        SocketMessage message, CommandInfo commandInfo)
    {
        var guildUser = message.Author as SocketGuildUser;
        if (guildUser == null && (commandInfo.GuildPermissions.Any() || commandInfo.ChannelPermissions.Any()))
        {
            return (false, "❌ This command can only be used in a server.");
        }

        // Validate required guild permissions
        foreach (var guildPerm in commandInfo.GuildPermissions)
        {
            if (guildUser != null && !guildUser.GuildPermissions.Has(guildPerm.Permission))
            {
                return (false, $"❌ You need `{guildPerm.Permission}` permission to use this command.");
            }
        }

        // Validate required channel permissions
        foreach (var channelPerm in commandInfo.ChannelPermissions)
        {
            if (guildUser != null)
            {
                var channelPerms = guildUser.GetPermissions(message.Channel as IGuildChannel);
                if (!channelPerms.Has(channelPerm.Permission))
                {
                    return (false, $"❌ You need `{channelPerm.Permission}` permission in this channel.");
                }
            }
        }

        return (true, null);
    }

    /// <summary>
    /// Sends an error message as an embed reply.
    /// </summary>
    private async Task SendErrorMessage(SocketMessage message, string errorMessage)
    {
        if (message.Channel is IMessageChannel channel)
        {
            var embed = new EmbedBuilder()
                .WithDescription(errorMessage)
                .WithColor(Color.Red)
                .Build();

            await channel.SendMessageAsync(text: message.Author.Mention, embed: embed);
        }
    }
}
