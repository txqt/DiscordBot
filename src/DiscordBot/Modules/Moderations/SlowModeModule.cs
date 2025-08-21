using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot.Modules.Moderations;
public class SlowModeModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("slowmode", "Đặt slowmode cho channel")]
    [RequireUserPermission(Discord.ChannelPermission.ManageChannels)]
    [RequireBotPermission(Discord.ChannelPermission.ManageChannels)]
    public async Task SlowmodeAsync(
    [Summary("seconds", "Thời gian slowmode (giây)")] int seconds)
    {
        var channel = (SocketTextChannel)Context.Channel;

        await channel.ModifyAsync(prop => prop.SlowModeInterval = seconds);
        await RespondAsync($"Slowmode được đặt thành {seconds} giây!");
    }

}
