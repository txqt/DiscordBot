namespace DiscordBot.Models;

public class BotConfig
{
    public string Token { get; set; } = string.Empty;
    public string Prefix { get; set; } = "@";
    public ulong? GuildId { get; set; }
}
