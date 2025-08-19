using System.Reflection;
using DiscordBot.Attributes;
using DiscordBot.Commands;
using DiscordBot.Models;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Services;

/// <summary>
/// Handles discovery and registration of all bot commands using reflection.
/// </summary>
public class CommandRegistryService
{
    private readonly Dictionary<string, CommandInfo> _commands = new();
    private readonly ILogger<CommandRegistryService> _logger;

    public CommandRegistryService(ILogger<CommandRegistryService> logger)
    {
        _logger = logger;
        LoadCommands();
    }

    /// <summary>
    /// Gets a command by name, or null if not found.
    /// </summary>
    public CommandInfo? GetCommand(string name)
    {
        return _commands.TryGetValue(name.ToLower(), out var command) ? command : null;
    }

    /// <summary>
    /// Returns all registered commands.
    /// </summary>
    public IEnumerable<CommandInfo> GetAllCommands()
    {
        return _commands.Values;
    }

    /// <summary>
    /// Loads all command classes from the current assembly and registers them.
    /// </summary>
    private void LoadCommands()
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Find all non-abstract subclasses of BaseCommand
        var commandTypes = assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(BaseCommand)) && !t.IsAbstract)
            .ToList();

        foreach (var type in commandTypes)
        {
            var commandAttribute = type.GetCustomAttribute<DiscordCommandAttribute>();
            if (commandAttribute == null)
            {
                _logger.LogWarning("Command type {Type} is missing DiscordCommandAttribute", type.Name);
                continue;
            }

            // Collect required permissions if specified
            var guildPermissions = type.GetCustomAttributes<RequireGuildPermissionAttribute>().ToList();
            var channelPermissions = type.GetCustomAttributes<RequireChannelPermissionAttribute>().ToList();

            var commandInfo = new CommandInfo
            {
                Name = commandAttribute.Name,
                Description = commandAttribute.Description,
                CommandType = type,
                GuildPermissions = guildPermissions,
                ChannelPermissions = channelPermissions
            };

            _commands[commandAttribute.Name.ToLower()] = commandInfo;
            _logger.LogInformation("Registered command: {Command}", commandAttribute.Name);
        }

        _logger.LogInformation("Loaded {Count} commands", _commands.Count);
    }
}
