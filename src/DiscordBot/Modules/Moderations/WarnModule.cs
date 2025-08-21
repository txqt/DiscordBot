using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Models;

namespace DiscordBot.Modules.Moderations;

public class WarnModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("warn", "Cảnh cáo một thành viên")]
    [RequireUserPermission(GuildPermission.KickMembers)]
    public async Task WarnAsync(
        [Summary(description: "Người cần cảnh cáo")] SocketGuildUser user,
        [Summary(description: "Lý do cảnh cáo")] string reason = "Không có lý do")
    {
        if (user == null)
        {
            await RespondAsync("❌ Không tìm thấy user.", ephemeral: true);
            return;
        }

        // Lưu cảnh cáo
        if (!WarnStorage.Warnings.ContainsKey(user.Id))
            WarnStorage.Warnings[user.Id] = new List<string>();

        WarnStorage.Warnings[user.Id].Add(reason);

        var warnCount = WarnStorage.Warnings[user.Id].Count;

        // Kiểm tra số lần cảnh cáo
        if (warnCount >= 3)
        {
            try
            {
                await user.KickAsync($"Quá 3 cảnh cáo (tổng: {warnCount})");
                await RespondAsync($"⚠️ {user.Username} đã bị kick vì có {warnCount} cảnh cáo!");
            }
            catch (Exception ex)
            {
                await RespondAsync($"❌ Kick thất bại: {ex.Message}", ephemeral: true);
            }
        }
        else
        {
            await RespondAsync($"⚠️ {user.Username} đã bị cảnh cáo ({warnCount}/3). Lý do: {reason}");
        }
    }
}
