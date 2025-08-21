using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Modules.Moderations;

public class KickModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("kick", "Kick một thành viên khỏi server")]
    [RequireUserPermission(GuildPermission.KickMembers)]   // yêu cầu user gọi lệnh có quyền kick
    [RequireBotPermission(GuildPermission.KickMembers)]    // yêu cầu bot có quyền kick
    public async Task KickAsync(
        [Summary(description: "Người dùng cần kick")] SocketGuildUser user,
        [Summary(description: "Lý do kick (tùy chọn)")] string? reason = null)
    {
        if (user.Id == Context.User.Id)
        {
            await RespondAsync("❌ Bạn không thể kick chính mình!", ephemeral: true);
            return;
        }

        var bot = Context.Guild.CurrentUser;

        // check role của bot và user target
        if (bot.Hierarchy <= user.Hierarchy)
        {
            await RespondAsync("❌ Không thể kick người có role cao hơn hoặc bằng bot.", ephemeral: true);
            return;
        }

        try
        {
            await user.KickAsync(reason ?? "Không có lý do");
            await RespondAsync($"✅ Đã kick {user.Username} (Lý do: {reason ?? "Không có lý do"})");
        }
        catch (Exception ex)
        {
            await RespondAsync($"❌ Kick thất bại: {ex.Message}", ephemeral: true);
        }
    }
}
