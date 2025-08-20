using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Attributes;

namespace DiscordBot.Commands.Generals;
[DiscordCommand("userinfo", "Lấy thông tin người dùng")]
public class UserinfoCommand : BaseCommand
{
    private readonly DiscordSocketClient _client;

    public UserinfoCommand(DiscordSocketClient client)
    {
        _client = client;
    }

    public override async Task ExecuteAsync(SocketMessage message, string[] args)
    {
        var sm = message as SocketUserMessage;
        if (sm == null) return;

        var mentionedUser = sm.MentionedUsers.FirstOrDefault(u => u.Id != _client.CurrentUser.Id);
        var user = mentionedUser ?? message.Author;

        string? avatarUrl = null;
        if (user is SocketUser su)
            avatarUrl = su.GetAvatarUrl() ?? su.GetDefaultAvatarUrl();

        var embed = new EmbedBuilder()
            .WithTitle($"👤 Thông tin của {user.Username}")
            .WithThumbnailUrl(avatarUrl)
            .AddField("Tên", user.Username, true)
            .AddField("Tag", $"#{user.Discriminator}", true)
            .AddField("ID", user.Id.ToString(), true)
            .AddField("Tạo tài khoản", user.CreatedAt.ToString("dd/MM/yyyy HH:mm"), true)
            .WithColor(Color.Green)
            .Build();

        await ReplyAsync(message, embed: embed);
    }
}
