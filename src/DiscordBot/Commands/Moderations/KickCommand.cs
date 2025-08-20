using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Attributes;

namespace DiscordBot.Commands.Moderations;

[DiscordCommand("kick", "Kick một thành viên khỏi server")]
[RequireGuildPermission(Discord.GuildPermission.KickMembers)]
public class KickCommand : BaseCommand
{
    private readonly DiscordSocketClient _client;

    public KickCommand(DiscordSocketClient client)
    {
        _client = client;
    }

    public override async Task ExecuteAsync(SocketMessage message, string[] args)
    {
        if (args.Length < 1)
        {
            await ReplyAsync(message, "❌ Bạn cần mention 1 user để kick!");
            return;
        }

        var mentionedUser = message.MentionedUsers.FirstOrDefault(u => u.Id != _client.CurrentUser.Id);
        if (mentionedUser == null)
        {
            await ReplyAsync(message, "❌ Không tìm thấy user cần kick.");
            return;
        }

        var guildChannel = message.Channel as SocketGuildChannel;
        if (guildChannel == null)
        {
            await ReplyAsync(message, "❌ Lệnh này chỉ dùng trong server.");
            return;
        }
        var guild = guildChannel.Guild;

        var guildUser = mentionedUser as SocketGuildUser;
        if (guildUser == null)
            guildUser = guild.GetUser(mentionedUser.Id);

        if (guildUser == null)
        {
            try
            {
                var restUser = await _client.Rest.GetGuildUserAsync(guild.Id, mentionedUser.Id);
                if (restUser != null)
                {
                    // Kiểm tra quyền bot/author vẫn giống như khi dùng SocketGuildUser
                    var author = message.Author as SocketGuildUser;
                    var botUser = guild.CurrentUser;
                    if (author == null || !author.GuildPermissions.KickMembers)
                    {
                        await ReplyAsync(message, "❌ Bạn không có quyền kick!");
                        return;
                    }
                    if (botUser == null || !botUser.GuildPermissions.KickMembers)
                    {
                        await ReplyAsync(message, "❌ Bot không có quyền kick!");
                        return;
                    }

                    var reason = args.Length > 1 ? string.Join(" ", args.Skip(1)) : "Không có lý do";
                    await restUser.KickAsync(reason);
                    await ReplyAsync(message, $"✅ Đã kick {mentionedUser.Username} (Lý do: {reason})");
                    return;
                }
            }
            catch (Discord.Net.HttpException httpEx)
            {
                await ReplyAsync(message, $"❌ Lỗi khi lấy user bằng REST: {httpEx.Message}");
                return;
            }
            catch (Exception ex)
            {
                await ReplyAsync(message, $"❌ Lỗi không xác định: {ex.Message}");
                return;
            }

            await ReplyAsync(message, "❌ Không thể tìm user trong server (cache trống và REST trả về null).");
            return;
        }

        if (guildUser.Id == message.Author.Id)
        {
            await ReplyAsync(message, "❌ Bạn không thể kick chính mình!");
            return;
        }

        var botSocket = guild.CurrentUser;
        if (botSocket == null || !botSocket.GuildPermissions.KickMembers)
        {
            await ReplyAsync(message, "❌ Bot không có quyền kick!");
            return;
        }

        if (botSocket.Hierarchy <= guildUser.Hierarchy)
        {
            await ReplyAsync(message, "❌ Không thể kick người dùng có role cao hơn hoặc bằng bot.");
            return;
        }

        var reason2 = args.Length > 1 ? string.Join(" ", args.Skip(1)) : "Không có lý do";
        try
        {
            await guildUser.KickAsync(reason2);
            await ReplyAsync(message, $"✅ Đã kick {mentionedUser.Username} (Lý do: {reason2})");
        }
        catch (Exception ex)
        {
            await ReplyAsync(message, $"❌ Kick thất bại: {ex.Message}");
        }
    }
}
