using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Modules.Moderations;

public class MuteModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("mute", "Mute (timeout) một thành viên trong một khoảng thời gian (phút)")]
    [RequireUserPermission(GuildPermission.ModerateMembers)]
    [RequireBotPermission(GuildPermission.ModerateMembers)]
    public async Task MuteAsync(
        [Summary(description: "Người cần mute")] SocketGuildUser user,
        [Summary(description: "Thời gian mute (phút)")] int minutes,
        [Summary(description: "Lý do (tùy chọn)")] string? reason = null)
    {
        if (user.Id == Context.User.Id)
        {
            await RespondAsync("❌ Bạn không thể mute chính mình!", ephemeral: true);
            return;
        }

        if (minutes <= 0)
        {
            await RespondAsync("❌ Số phút phải lớn hơn 0.", ephemeral: true);
            return;
        }

        var bot = Context.Guild.CurrentUser;
        if (bot.Hierarchy <= user.Hierarchy)
        {
            await RespondAsync("❌ Không thể mute người có role cao hơn hoặc bằng bot.", ephemeral: true);
            return;
        }

        try
        {
            await user.SetTimeOutAsync(TimeSpan.FromMinutes(minutes), new RequestOptions
            {
                AuditLogReason = reason ?? "Không có lý do"
            });

            await RespondAsync(
                $"✅ Đã mute {user.Username} trong {minutes} phút. (Lý do: {reason ?? "Không có lý do"})"
            );
        }
        catch (Exception ex)
        {
            await RespondAsync($"❌ Mute thất bại: {ex.Message}", ephemeral: true);
        }
    }
}
