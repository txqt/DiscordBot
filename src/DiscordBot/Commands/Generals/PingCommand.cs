using Discord;
using Discord.WebSocket;
using DiscordBot.Attributes;

namespace DiscordBot.Commands.Generals;

/// <summary>
/// A simple command to check the bot's latency.
/// </summary>
[DiscordCommand("ping", "Check the bot's latency")]
public class PingCommand : BaseCommand
{
    private readonly DiscordSocketClient _client;

    public PingCommand(DiscordSocketClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Replies with the bot's latency in milliseconds.
    /// </summary>
    public override async Task ExecuteAsync(SocketMessage message, string[] args)
    {
        var embed = new EmbedBuilder()
            .WithTitle("Pong!")
            .WithDescription($"Latency: {_client.Latency}ms")
            .WithColor(Color.Green)
            .Build();

        await ReplyAsync(message, embed: embed);
    }
}
