using Discord;

namespace DiscordBot.Attributes;

/// <summary>
/// Requires a specific guild permission to execute the command.
/// Can be applied multiple times to a command class.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequireGuildPermissionAttribute : Attribute
{
    /// <summary>
    /// The required guild permission.
    /// </summary>
    public GuildPermission Permission { get; }

    public RequireGuildPermissionAttribute(GuildPermission permission)
    {
        Permission = permission;
    }
}
