using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot.Modules.Moderations;
public class LockModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("lock", "Khóa channel này")]
    [RequireUserPermission(GuildPermission.ManageChannels)]
    [RequireBotPermission(GuildPermission.ManageChannels)]
    public async Task LockAsync()
    {
        var channel = (SocketTextChannel)Context.Channel;
        var everyone = channel.Guild.EveryoneRole;

        if (!((SocketGuildUser)Context.User).GuildPermissions.ManageChannels)
        {
            await RespondAsync("Bạn không có quyền khóa channel!", ephemeral: true);
            return;
        }

        await channel.AddPermissionOverwriteAsync(everyone, new OverwritePermissions(sendMessages: PermValue.Deny));
        await RespondAsync("Channel đã bị khóa 🔒");
    }

    [SlashCommand("unlock", "Mở khóa channel này")]
    [RequireUserPermission(GuildPermission.ManageChannels)]
    [RequireBotPermission(GuildPermission.ManageChannels)]
    public async Task UnlockAsync()
    {
        var channel = (SocketTextChannel)Context.Channel;
        var everyone = channel.Guild.EveryoneRole;

        if (!((SocketGuildUser)Context.User).GuildPermissions.ManageChannels)
        {
            await RespondAsync("Bạn không có quyền mở khóa channel!", ephemeral: true);
            return;
        }

        await channel.AddPermissionOverwriteAsync(everyone, new OverwritePermissions(sendMessages: PermValue.Allow));
        await RespondAsync("Channel đã được mở khóa 🔓");
    }

}
