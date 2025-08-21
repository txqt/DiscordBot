using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot.Modules.Moderations;

public class UnmuteModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("unmute", "Bỏ mute (timeout) một thành viên")]
    [RequireUserPermission(GuildPermission.ModerateMembers)]
    [RequireBotPermission(GuildPermission.ModerateMembers)]
    public async Task UnmuteAsync(
        [Summary(description: "Người cần unmute")] SocketGuildUser user)
    {
        if (user == null)
        {
            await RespondAsync("❌ Không tìm thấy user trong server.", ephemeral: true);
            return;
        }

        try
        {
            await user.RemoveTimeOutAsync();
            await RespondAsync($"✅ Đã unmute **{user.Username}**.");
        }
        catch (Exception ex)
        {
            await RespondAsync($"❌ Unmute thất bại: {ex.Message}", ephemeral: true);
        }
    }
}
