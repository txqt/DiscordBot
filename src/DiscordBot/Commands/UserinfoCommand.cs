using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Commands;
public class UserinfoCommand : BaseCommand
{
    public override async Task ExecuteAsync(SocketMessage message, string[] args)
    {
        var mentionedUser = (message as SocketUserMessage)?.MentionedUsers.FirstOrDefault();
        var user = mentionedUser ?? message.Author;

        var embed = new EmbedBuilder()
            .WithTitle($"👤 Thông tin của {user.Username}")
            .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .AddField("Tên", user.Username, true)
            .AddField("Tag", $"#{user.Discriminator}", true)
            .AddField("ID", user.Id, true)
            .AddField("Tạo tài khoản", user.CreatedAt.ToString("dd/MM/yyyy HH:mm"), true)
            .WithColor(Color.Green)
            .Build();

        await ReplyAsync(message, embed: embed);
    }
}
