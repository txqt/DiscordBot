﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot.Modules.Generals;

public class PingModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly DiscordSocketClient _client;

    public PingModule(DiscordSocketClient client)
    {
        _client = client;
    }

    [SlashCommand("ping", "Check the bot's latency")]
    public async Task PingAsync()
    {
        var embed = new EmbedBuilder()
            .WithTitle("Pong! 🏓")
            .WithDescription($"Latency: {_client.Latency}ms")
            .WithColor(Color.Green)
            .Build();

        await RespondAsync(embed: embed);
    }
}
