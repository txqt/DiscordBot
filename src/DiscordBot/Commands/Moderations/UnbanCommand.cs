using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Attributes;

namespace DiscordBot.Commands.Moderations;

[DiscordCommand("unban", "Unban một thành viên khỏi server")]
[RequireGuildPermission(GuildPermission.BanMembers)]
public class UnbanCommand : BaseCommand
{
    private readonly DiscordSocketClient _client;

    public UnbanCommand(DiscordSocketClient client)
    {
        _client = client;
    }

    public override async Task ExecuteAsync(SocketMessage message, string[] args)
    {
        if (!(message.Channel is SocketGuildChannel guildChannel))
        {
            await ReplyAsync(message, "❌ Lệnh này chỉ dùng trong server.");
            return;
        }

        var guild = guildChannel.Guild;

        if (args.Length < 1)
        {
            await ReplyAsync(message, "❌ Bạn cần nhập **ID** hoặc **username#discriminator** để unban!");
            return;
        }

        try
        {
            var bans = new List<RestBan>();

            await foreach (var banPage in guild.GetBansAsync())
            {
                bans.AddRange(banPage);
            }

            IUser? bannedUser = null;

            // TH1: nhập ID
            if (ulong.TryParse(args[0], out ulong userId))
            {
                bannedUser = bans.FirstOrDefault(b => b.User.Id == userId)?.User;
            }
            else
            {
                // TH2: nhập tên#tag
                var input = args[0];
                bannedUser = bans.FirstOrDefault(b =>
                    string.Equals($"{b.User.Username}#{b.User.Discriminator}", input, StringComparison.OrdinalIgnoreCase)
                )?.User;
            }

            if (bannedUser == null)
            {
                await ReplyAsync(message, "❌ Không tìm thấy user trong danh sách ban.");
                return;
            }

            await guild.RemoveBanAsync(bannedUser);

            var reason = args.Length > 1 ? string.Join(" ", args.Skip(1)) : "Không có lý do";
            await ReplyAsync(message, $"✅ Đã unban **{bannedUser.Username}#{bannedUser.Discriminator}** (Lý do: {reason})");
        }
        catch (Exception ex)
        {
            await ReplyAsync(message, $"❌ Unban thất bại: {ex.Message}");
        }
    }
}
