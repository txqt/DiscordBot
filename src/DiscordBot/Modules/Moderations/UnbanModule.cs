using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot.Modules.Moderations;

public class UnbanModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("unban", "Unban một thành viên khỏi server bằng ID")]
    [RequireUserPermission(GuildPermission.BanMembers)]
    [RequireBotPermission(GuildPermission.BanMembers)]
    public async Task UnbanAsync(
        [Summary(description: "ID của user cần unban")] ulong userId,
        [Summary(description: "Lý do (tùy chọn)")] string? reason = null)
    {
        try
        {
            var guild = Context.Guild;

            // Lấy danh sách ban
            var bans = await guild.GetBansAsync().FlattenAsync();
            var bannedUser = bans.FirstOrDefault(b => b.User.Id == userId)?.User;

            if (bannedUser == null)
            {
                await RespondAsync("❌ Không tìm thấy user trong danh sách ban.", ephemeral: true);
                return;
            }

            // Gỡ ban
            await guild.RemoveBanAsync(bannedUser, new RequestOptions
            {
                AuditLogReason = reason ?? "Không có lý do"
            });

            await RespondAsync(
                $"✅ Đã unban **{bannedUser.Username}#{bannedUser.Discriminator}** (Lý do: {reason ?? "Không có lý do"})"
            );
        }
        catch (Exception ex)
        {
            await RespondAsync($"❌ Unban thất bại: {ex.Message}", ephemeral: true);
        }
    }
}
