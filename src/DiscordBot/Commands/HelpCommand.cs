using Discord;
using Discord.WebSocket;
using DiscordBot.Attributes;
using DiscordBot.Services;
using System.Text;

namespace DiscordBot.Commands;

[DiscordCommand("help", "Hiển thị danh sách các lệnh có sẵn")]
public class HelpCommand : BaseCommand
{
    private readonly CommandRegistryService _commandRegistry;
    private readonly DiscordSocketClient _client;

    public HelpCommand(CommandRegistryService commandRegistry, DiscordSocketClient client)
    {
        _commandRegistry = commandRegistry;
        _client = client;
    }

    public override async Task ExecuteAsync(SocketMessage message, string[] args)
    {
        var commands = _commandRegistry.GetAllCommands().OrderBy(c => c.Name);
        var guildUser = message.Author as SocketGuildUser;

        var embed = new EmbedBuilder()
            .WithTitle("📋 Danh sách lệnh")
            .WithDescription($"Sử dụng: {_client.CurrentUser.Mention} <lệnh> [tham số]")
            .WithColor(Color.Blue)
            .WithTimestamp(DateTimeOffset.Now);

        var availableCommands = new StringBuilder();
        var restrictedCommands = new StringBuilder();

        foreach (var cmd in commands)
        {
            var permissions = GetPermissionText(cmd);
            var commandText = $"`{cmd.Name}` - {cmd.Description}";

            if (!string.IsNullOrEmpty(permissions))
                commandText += $"\n   *Yêu cầu: {permissions}*";

            // Check if user has permissions for this command
            if (HasUserPermission(guildUser, cmd))
            {
                availableCommands.AppendLine(commandText);
            }
            else
            {
                restrictedCommands.AppendLine(commandText);
            }
        }

        if (availableCommands.Length > 0)
        {
            embed.AddField("✅ Lệnh có thể sử dụng", availableCommands.ToString(), false);
        }

        if (restrictedCommands.Length > 0)
        {
            embed.AddField("🔒 Lệnh bị hạn chế", restrictedCommands.ToString(), false);
        }

        await ReplyAsync(message, embed.Build());
    }

    private string GetPermissionText(Models.CommandInfo cmd)
    {
        var permissions = new List<string>();

        foreach (var guildPerm in cmd.GuildPermissions)
        {
            permissions.Add($"Guild: {guildPerm.Permission}");
        }

        foreach (var channelPerm in cmd.ChannelPermissions)
        {
            permissions.Add($"Channel: {channelPerm.Permission}");
        }

        return string.Join(", ", permissions);
    }

    private bool HasUserPermission(SocketGuildUser? guildUser, Models.CommandInfo cmd)
    {
        if (guildUser == null && (cmd.GuildPermissions.Any() || cmd.ChannelPermissions.Any()))
            return false;

        // Check guild permissions
        foreach (var guildPerm in cmd.GuildPermissions)
        {
            if (guildUser != null && !guildUser.GuildPermissions.Has(guildPerm.Permission))
                return false;
        }

        // For channel permissions, we can't check specific channel here, so assume they have it
        // The actual check will happen when they try to use the command

        return true;
    }
}
