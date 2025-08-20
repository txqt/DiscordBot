using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Attributes;

namespace DiscordBot.Commands.Moderations;

[DiscordCommand("mute", "Mute (timeout) một thành viên")]
[RequireGuildPermission(GuildPermission.ModerateMembers)]
public class MuteCommand : BaseCommand
{
    private readonly DiscordSocketClient _client;

    public MuteCommand(DiscordSocketClient client)
    {
        _client = client;
    }

    public override async Task ExecuteAsync(SocketMessage message, string[] args)
    {
        if (!(message.Channel is SocketGuildChannel guildChannel))
        {
            await ReplyAsync(message, "❌ Lệnh này chỉ dùng trong server.");
            return;
        }
        var guild = guildChannel.Guild;

        if (args.Length < 2)
        {
            await ReplyAsync(message, "❌ Cú pháp: `@Bot mute @user <số phút>`");
            return;
        }

        var mentionedUser = message.MentionedUsers.FirstOrDefault(u => u.Id != _client.CurrentUser.Id);
        if (mentionedUser == null)
        {
            await ReplyAsync(message, "❌ Bạn cần mention 1 user.");
            return;
        }

        if (!int.TryParse(args[1], out int minutes) || minutes <= 0)
        {
            await ReplyAsync(message, "❌ Số phút không hợp lệ.");
            return;
        }

        var guildUser = guild.GetUser(mentionedUser.Id);
        if (guildUser == null)
        {
            await ReplyAsync(message, "❌ Không tìm thấy user trong server.");
            return;
        }

        await guildUser.SetTimeOutAsync(TimeSpan.FromMinutes(minutes));
        await ReplyAsync(message, $"✅ Đã mute {guildUser.Username} trong {minutes} phút.");
    }
}
