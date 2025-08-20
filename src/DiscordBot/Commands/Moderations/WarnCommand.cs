using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Attributes;
using DiscordBot.Models;

namespace DiscordBot.Commands.Moderations;

[DiscordCommand("warn", "Cảnh cáo một thành viên")]
[RequireGuildPermission(GuildPermission.KickMembers)]
public class WarnCommand : BaseCommand
{
    private readonly DiscordSocketClient _client;

    public WarnCommand(DiscordSocketClient client)
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
            await ReplyAsync(message, "❌ Bạn cần mention 1 user để cảnh cáo.");
            return;
        }

        var reason = args.Length > 1 ? string.Join(" ", args.Skip(1)) : "Không có lý do";

        if (!WarnStorage.Warnings.ContainsKey(mentionedUser.Id))
            WarnStorage.Warnings[mentionedUser.Id] = new List<string>();

        WarnStorage.Warnings[mentionedUser.Id].Add(reason);

        var warnCount = WarnStorage.Warnings[mentionedUser.Id].Count;

        if (warnCount >= 3)
        {
            // Kick user
            var guildUser = guild.GetUser(mentionedUser.Id);
            if (guildUser != null)
            {
                try
                {
                    await guildUser.KickAsync($"Quá 3 cảnh cáo (tổng: {warnCount})");
                    await ReplyAsync(message, $"⚠️ {mentionedUser.Username} đã bị kick vì có {warnCount} cảnh cáo!");
                }
                catch (Exception ex)
                {
                    await ReplyAsync(message, $"❌ Kick thất bại: {ex.Message}");
                }
            }
            else
            {
                await ReplyAsync(message, $"❌ Không tìm thấy {mentionedUser.Username} trong server để kick.");
            }
        }
        else
        {
            await ReplyAsync(message, $"⚠️ {mentionedUser.Username} đã bị cảnh cáo ({warnCount}/3). Lý do: {reason}");
        }
    }
}
