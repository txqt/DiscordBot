using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Attributes;

namespace DiscordBot.Commands.Generals;

[DiscordCommand("about", "Thông tin về bot")]
public class AboutCommand : BaseCommand
{
    private readonly DiscordSocketClient _client;
    private readonly DateTime _startTime;
    public AboutCommand(DiscordSocketClient client)
    {
        _client = client;
        _startTime = DateTime.Now;
    }

    public override async Task ExecuteAsync(SocketMessage message, string[] args)
    {
        var botUser = _client.CurrentUser;

        var embed = new EmbedBuilder()
            .WithTitle("🤖 About this bot")
            .WithThumbnailUrl(botUser.GetAvatarUrl() ?? botUser.GetDefaultAvatarUrl())
            .AddField("Tên", botUser.Username, true)
            .AddField("Tag", botUser.Discriminator, true)
            .AddField("ID", botUser.Id, true)
            .AddField("Tạo ngày", botUser.CreatedAt.ToString("dd/MM/yyyy"), true)
            .AddField("Trạng thái", _client.Status.ToString(), true)
            .AddField("Uptime", (DateTime.Now - _startTime).ToString(@"dd\.hh\:mm\:ss"))
            .WithFooter($"Yêu cầu bởi {message.Author.Username}")
            .WithColor(Color.Blue)
            .Build();

        await ReplyAsync(message, embed: embed);
    }
}
