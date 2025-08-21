using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot.Modules.Moderations;
public class PurgeUser : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("purge_user", "Xóa tin nhắn của một thành viên")]
    [RequireUserPermission(GuildPermission.ManageMessages)]
    [RequireBotPermission(GuildPermission.ManageMessages)]
    public async Task PurgeUserAsync(
    [Summary("user", "Người dùng cần xóa tin nhắn")] SocketGuildUser user,
    [Summary("amount", "Số lượng tin nhắn muốn xóa")] int amount = 10)
    {
        var channel = (SocketTextChannel)Context.Channel;

        if (!((SocketGuildUser)Context.User).GuildPermissions.ManageMessages)
        {
            await RespondAsync("Bạn không có quyền xóa tin nhắn!", ephemeral: true);
            return;
        }

        var messages = await channel.GetMessagesAsync(100).FlattenAsync();
        var userMessages = messages.Where(m => m.Author.Id == user.Id).Take(amount);

        await channel.DeleteMessagesAsync(userMessages);
        await RespondAsync($"Đã xóa {userMessages.Count()} tin nhắn của {user.Mention}");
    }

}
