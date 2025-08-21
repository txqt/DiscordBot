using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot.Modules.Generals;

public class AvatarModule : InteractionModuleBase<SocketInteractionContext>
{

    [SlashCommand("avatar", "Xem avatar của một user")]
    public async Task AvatarAsync(
        [Summary("user", "Người dùng muốn xem avatar (nếu bỏ trống sẽ lấy của bạn)")]
        SocketUser? user = null)
    {
        user ??= Context.User;

        var embed = new EmbedBuilder()
            .WithTitle($"Avatar của {user.Username}")
            .WithImageUrl(user.GetAvatarUrl(size: 1024) ?? user.GetDefaultAvatarUrl())
            .WithColor(Color.Blue)
            .Build();

        await RespondAsync(embed: embed);
    }
}

