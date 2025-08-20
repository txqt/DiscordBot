using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Attributes;

namespace DiscordBot.Commands.Moderations;

[DiscordCommand("ban", "Ban một thành viên khỏi server")]
[RequireGuildPermission(Discord.GuildPermission.BanMembers)]
public class BanCommand : BaseCommand
{
    private readonly DiscordSocketClient _client;

    public BanCommand(DiscordSocketClient client)
    {
        _client = client;
    }

    public override async Task ExecuteAsync(SocketMessage message, string[] args)
    {
        if (args.Length < 1)
        {
            await ReplyAsync(message, "❌ Bạn cần mention 1 user để ban!");
            return;
        }

        var mentionedUser = message.MentionedUsers.FirstOrDefault(u => u.Id != _client.CurrentUser.Id);
        if (mentionedUser == null)
        {
            await ReplyAsync(message, "❌ Không tìm thấy user cần ban.");
            return;
        }

        if (!(message.Channel is SocketGuildChannel guildChannel))
        {
            await ReplyAsync(message, "❌ Lệnh này chỉ dùng trong server.");
            return;
        }
        var guild = guildChannel.Guild;

        var guildUser = mentionedUser as SocketGuildUser ?? guild.GetUser(mentionedUser.Id);

        if (guildUser == null)
        {
            await ReplyAsync(message, "❌ Không thể tìm user trong server (cache trống).");
            return;
        }

        if (guildUser.Id == message.Author.Id)
        {
            await ReplyAsync(message, "❌ Bạn không thể ban chính mình!");
            return;
        }

        var botSocket = guild.CurrentUser;
        if (botSocket == null || !botSocket.GuildPermissions.BanMembers)
        {
            await ReplyAsync(message, "❌ Bot không có quyền ban!");
            return;
        }

        if (botSocket.Hierarchy <= guildUser.Hierarchy)
        {
            await ReplyAsync(message, "❌ Không thể ban người có role cao hơn hoặc bằng bot.");
            return;
        }

        var reason2 = args.Length > 1 ? string.Join(" ", args.Skip(1)) : "Không có lý do";
        try
        {
            // pruneDays = 0 (không xoá tin nhắn cũ), có thể chỉnh 1–7 ngày nếu muốn
            await guild.AddBanAsync(guildUser, pruneDays: 0, reason: reason2);
            await ReplyAsync(message, $"✅ Đã ban {mentionedUser.Username} (Lý do: {reason2})");
        }
        catch (Exception ex)
        {
            await ReplyAsync(message, $"❌ Ban thất bại: {ex.Message}");
        }
    }
}
