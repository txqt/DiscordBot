using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Modules.Moderations;

public class BanModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ban", "Ban một thành viên khỏi server")]
    [RequireUserPermission(GuildPermission.BanMembers)]
    [RequireBotPermission(GuildPermission.BanMembers)]
    public async Task BanAsync(
        SocketGuildUser user,
        string? reason = null)
    {
        reason ??= "Không có lý do";

        var guild = Context.Guild;
        var botUser = guild.CurrentUser;

        // Check: không cho tự ban mình
        if (user.Id == Context.User.Id)
        {
            await RespondAsync("❌ Bạn không thể ban chính mình!", ephemeral: true);
            return;
        }

        // Check role position
        if (botUser.Hierarchy <= user.Hierarchy)
        {
            await RespondAsync("❌ Bot không thể ban người có role cao hơn hoặc bằng mình.", ephemeral: true);
            return;
        }

        try
        {
            await guild.AddBanAsync(user, pruneDays: 0, reason: reason);
            await RespondAsync($"✅ Đã ban **{user.Username}** (Lý do: {reason})");
        }
        catch (Exception ex)
        {
            await RespondAsync($"❌ Ban thất bại: {ex.Message}", ephemeral: true);
        }
    }
}
