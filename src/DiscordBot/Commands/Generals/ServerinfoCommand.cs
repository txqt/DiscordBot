using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Attributes;

namespace DiscordBot.Commands.Generals;

[DiscordCommand("serverinfo", "Xem thông tin server hiện tại")]
public class ServerinfoCommand : BaseCommand
{
    public override async Task ExecuteAsync(SocketMessage message, string[] arg)
    {
        var guild = (message.Channel as SocketGuildChannel)?.Guild;
        if (guild == null)
        {
            await message.Channel.SendMessageAsync("Lệnh này chỉ dùng trong server.");
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

        await message.Channel.SendMessageAsync(embed: embed);
    }
}
