using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Attributes;

namespace DiscordBot.Commands.Generals;

[DiscordCommand("avatar", "Xem avatar của một user")]
public class AvatarCommand : BaseCommand
{
    private readonly DiscordSocketClient _client;
    public AvatarCommand(DiscordSocketClient client)
    {
        _client = client;
    }
    public override async Task ExecuteAsync(SocketMessage message, string[] args)
    {
        var mentionedUser = (message as SocketUserMessage)?.MentionedUsers.FirstOrDefault(u => u.Id != _client.CurrentUser.Id);
        var user = mentionedUser ?? message.Author;

        var embed = new EmbedBuilder()
            .WithTitle($"Avatar của {user.Username}")
            .WithImageUrl(user.GetAvatarUrl(size: 1024) ?? user.GetDefaultAvatarUrl())
            .WithColor(Color.Blue)
            .Build();

        await message.Channel.SendMessageAsync(embed: embed);
    }
}

