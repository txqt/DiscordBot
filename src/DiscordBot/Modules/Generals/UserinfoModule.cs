using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordBot.Modules.Generals;

public class UserinfoModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("userinfo", "Lấy thông tin người dùng")]
    public async Task UserInfoAsync(SocketUser? user = null)
    {
        user ??= Context.User; // nếu không truyền thì lấy người gọi lệnh

        var avatarUrl = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();

        var embed = new EmbedBuilder()
            .WithTitle($"👤 Thông tin của {user.Username}")
            .WithThumbnailUrl(avatarUrl)
            .AddField("Tên", user.Username, true)
            .AddField("Tag", $"#{user.Discriminator}", true)
            .AddField("ID", user.Id.ToString(), true)
            .AddField("Tạo tài khoản", user.CreatedAt.ToString("dd/MM/yyyy HH:mm"), true)
            .WithColor(Color.Green)
            .Build();

        await RespondAsync(embed: embed);
    }
}
