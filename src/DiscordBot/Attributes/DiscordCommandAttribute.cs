namespace DiscordBot.Attributes;

/// <summary>
/// Marks a class as a Discord command and provides its metadata.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DiscordCommandAttribute : Attribute
{
    /// <summary>
    /// Command name used to trigger this command.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Short description of what the command does.
    /// </summary>
    public string Description { get; }

    public DiscordCommandAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
