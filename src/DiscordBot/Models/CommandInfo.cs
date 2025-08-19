using DiscordBot.Attributes;

namespace DiscordBot.Models;

public class CommandInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Type CommandType { get; set; } = null!;
    public List<RequireGuildPermissionAttribute> GuildPermissions { get; set; } = new();
    public List<RequireChannelPermissionAttribute> ChannelPermissions { get; set; } = new();
}
