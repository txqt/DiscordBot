using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Models;

namespace DiscordBot.Modules.Moderations;

public class WarningsModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("warnings", "Xem cảnh cáo của một thành viên")]
    public async Task WarningsAsync(
        [Summary(description: "Người cần xem cảnh cáo")] SocketGuildUser? user = null)
    {
        // Nếu không truyền user → mặc định là chính người gọi lệnh
        user ??= Context.User as SocketGuildUser;

        if (user == null)
        {
            await RespondAsync("❌ Không tìm thấy user.", ephemeral: true);
            return;
        }

        if (!WarnStorage.Warnings.ContainsKey(user.Id) || WarnStorage.Warnings[user.Id].Count == 0)
        {
            await RespondAsync($"✅ {user.Username} chưa có cảnh cáo nào.");
            return;
        }

        var warns = WarnStorage.Warnings[user.Id];
        var list = string.Join("\n", warns.Select((w, i) => $"{i + 1}. {w}"));

        var embed = new EmbedBuilder()
            .WithTitle($"⚠️ Cảnh cáo của {user.Username}")
            .WithDescription(list)
            .WithColor(Color.Orange)
            .Build();

        await RespondAsync(embed: embed);
    }
}
