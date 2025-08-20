using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Attributes;

namespace DiscordBot.Commands.Moderations;

[DiscordCommand("unmute", "Bỏ mute (timeout) một thành viên")]
[RequireGuildPermission(GuildPermission.ModerateMembers)]
public class UnmuteCommand : BaseCommand
{
    private readonly DiscordSocketClient _client;
    public UnmuteCommand(DiscordSocketClient client)
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

        var mentionedUser = message.MentionedUsers.FirstOrDefault(u => u.Id != _client.CurrentUser.Id);
        if (mentionedUser == null)
        {
            await ReplyAsync(message, "❌ Bạn cần mention 1 user.");
            return;
        }

        var guildUser = guild.GetUser(mentionedUser.Id);
        if (guildUser == null)
        {
            await ReplyAsync(message, "❌ Không tìm thấy user trong server.");
            return;
        }

        await guildUser.RemoveTimeOutAsync();
        await ReplyAsync(message, $"✅ Đã unmute {guildUser.Username}.");
    }
}
