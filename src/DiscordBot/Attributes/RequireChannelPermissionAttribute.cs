using Discord;

namespace DiscordBot.Attributes;

/// <summary>
/// Requires a specific channel permission to execute the command.
/// Can be applied multiple times to a command class.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequireChannelPermissionAttribute : Attribute
{
    /// <summary>
    /// The required channel permission.
    /// </summary>
    public ChannelPermission Permission { get; }

    public RequireChannelPermissionAttribute(ChannelPermission permission)
    {
        Permission = permission;
    }
}
