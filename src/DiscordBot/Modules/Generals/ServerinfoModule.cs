using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordBot.Modules.Generals;

public class ServerinfoModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("serverinfo", "Xem thông tin server hiện tại")]
    public async Task ServerInfoAsync()
    {
        var guild = Context.Guild;
        if (guild == null)
        {
            await RespondAsync("❌ Lệnh này chỉ dùng trong server.", ephemeral: true);
            return;
        }

        var owner = guild.Owner;
        var textChannels = guild.TextChannels.Count;
        var voiceChannels = guild.VoiceChannels.Count;
        var members = guild.MemberCount;

        var embed = new EmbedBuilder()
            .WithTitle($"📌 Thông tin server: {guild.Name}")
            .WithThumbnailUrl(guild.IconUrl)
            .AddField("👑 Chủ server", owner?.Username ?? "Không rõ", true)
            .AddField("🆔 Server ID", guild.Id, true)
            .AddField("👥 Thành viên", members, true)
            .AddField("💬 Text Channels", textChannels, true)
            .AddField("🎤 Voice Channels", voiceChannels, true)
            .AddField("📅 Tạo ngày", guild.CreatedAt.ToString("dd/MM/yyyy"), true)
            .WithColor(Color.Blue)
            .Build();

        await RespondAsync(embed: embed);
    }

}
