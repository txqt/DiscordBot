using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Attributes;

namespace DiscordBot.Commands.Moderations;

[DiscordCommand("clear", "Xóa một số lượng tin nhắn trong kênh")]
[RequireGuildPermission(GuildPermission.ManageMessages)]
public class ClearCommand : BaseCommand
{
    public override async Task ExecuteAsync(SocketMessage message, string[] args)
    {
        if (!(message.Channel is SocketTextChannel textChannel))
        {
            await ReplyAsync(message, "❌ Lệnh này chỉ dùng trong kênh text.");
            return;
        }

        if (args.Length < 1 || !int.TryParse(args[0], out int count) || count <= 0)
        {
            await ReplyAsync(message, "❌ Cú pháp: `@Bot clear <số tin>`");
            return;
        }

        var messages = await textChannel.GetMessagesAsync(limit: count + 1).FlattenAsync(); // +1 để xóa luôn lệnh
        await textChannel.DeleteMessagesAsync(messages);

        await ReplyAsync(message, $"✅ Đã xóa {count} tin nhắn.");
    }
}
