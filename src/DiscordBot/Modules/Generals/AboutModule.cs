using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot.Modules.Generals;

public class AboutModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly DiscordSocketClient _client;
    private static readonly DateTime _startTime = DateTime.Now;

    public AboutModule(DiscordSocketClient client)
    {
        _client = client;
    }

    [SlashCommand("about", "Thông tin về bot")]
    public async Task AboutAsync()
    {
        var botUser = _client.CurrentUser;
        var uptime = DateTime.Now - _startTime;

        var embed = new EmbedBuilder()
            .WithTitle("🤖 About this bot")
            .WithThumbnailUrl(botUser.GetAvatarUrl() ?? botUser.GetDefaultAvatarUrl())
            .AddField("Tên", botUser.Username, true)
            .AddField("Tag", botUser.Discriminator, true)
            .AddField("ID", botUser.Id, true)
            .AddField("Tạo ngày", botUser.CreatedAt.ToString("dd/MM/yyyy"), true)
            .AddField("Trạng thái", _client.Status.ToString(), true)
            .AddField("Uptime", $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s")
            .WithFooter($"Yêu cầu bởi {Context.User.Username}")
            .WithColor(Color.Blue)
            .Build();

        await RespondAsync(embed: embed);
    }
}
