using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Attributes;
using DiscordBot.Models;

namespace DiscordBot.Commands.Moderations;

[DiscordCommand("warnings", "Xem cảnh cáo của một thành viên")]
public class WarningsCommand : BaseCommand
{
    private readonly DiscordSocketClient _client;
    public WarningsCommand(DiscordSocketClient client)
    {
        _client = client;
    }
    public override async Task ExecuteAsync(SocketMessage message, string[] args)
    {
        var mentionedUser = message.MentionedUsers.FirstOrDefault(u => u.Id != _client.CurrentUser.Id) ?? message.Author;

        if (!WarnStorage.Warnings.ContainsKey(mentionedUser.Id) || WarnStorage.Warnings[mentionedUser.Id].Count == 0)
        {
            await ReplyAsync(message, $"✅ {mentionedUser.Username} chưa có cảnh cáo nào.");
            return;
        }

        var warns = WarnStorage.Warnings[mentionedUser.Id];
        var list = string.Join("\n", warns.Select((w, i) => $"{i + 1}. {w}"));
        await ReplyAsync(message, $"⚠️ Cảnh cáo của {mentionedUser.Username}:\n{list}");
    }
}
