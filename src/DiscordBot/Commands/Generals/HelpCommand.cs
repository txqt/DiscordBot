using Discord;
using Discord.WebSocket;
using DiscordBot.Attributes;
using DiscordBot.Services;
using System.Text;

namespace DiscordBot.Commands.Generals;

[DiscordCommand("help", "Hiển thị danh sách các lệnh có sẵn")]
public class HelpCommand : BaseCommand
{
    private readonly CommandRegistryService _commandRegistry;
    private readonly DiscordSocketClient _client;

    public HelpCommand(CommandRegistryService commandRegistry, DiscordSocketClient client)
    {
        _commandRegistry = commandRegistry;
        _client = client;
    }

    public override async Task ExecuteAsync(SocketMessage message, string[] args)
    {
        var commands = _commandRegistry.GetAllCommands().OrderBy(c => c.Name).ToList();
        var guildUser = message.Author as SocketGuildUser;

        var embed = new EmbedBuilder()
            .WithTitle("📋 Danh sách lệnh")
            .WithDescription($"Sử dụng: {_client.CurrentUser.Mention} <lệnh> [tham số]\n\nGõ `@Bot help <lệnh>` để xem chi tiết từng lệnh.")
            .WithColor(Color.Blue)
            .WithTimestamp(DateTimeOffset.Now);

        var availableCommands = new StringBuilder();
        var restricted = new List<Models.CommandInfo>();

        // Phân loại: có thể dùng / bị hạn chế
        foreach (var cmd in commands)
        {
            if (HasUserPermission(guildUser, cmd))
                AppendCommandLine(availableCommands, cmd);
            else
                restricted.Add(cmd);
        }

        // Thêm phần lệnh có thể dùng
        if (availableCommands.Length > 0)
            embed.AddField("✅ Lệnh có thể sử dụng", availableCommands.ToString(), false);

        // Gộp các lệnh bị hạn chế theo chuỗi yêu cầu (ví dụ "Guild: BanMembers, Channel: ManageMessages")
        if (restricted.Any())
        {
            // Nhóm theo permission text (cùng 1 tập quyền sẽ cùng nhóm)
            var grouped = restricted
                .GroupBy(c => GetPermissionText(c))
                .OrderBy(g => string.IsNullOrEmpty(g.Key) ? "0" : g.Key);

            foreach (var group in grouped)
            {
                var sb = new StringBuilder();
                foreach (var cmd in group.OrderBy(c => c.Name))
                {
                    sb.AppendLine($"• **{cmd.Name}** — {Truncate(cmd.Description, 80)}");
                }

                var header = string.IsNullOrEmpty(group.Key) ? "🔒 Lệnh bị hạn chế" : $"🔒 Lệnh bị hạn chế — _Yêu cầu: {group.Key}_";
                // Nếu nội dung quá dài cho field, cắt bớt để tránh lỗi (Discord embed field giới hạn)
                var content = sb.ToString();
                if (content.Length > 950) // chút đệm so với giới hạn 1024
                    content = content.Substring(0, 947) + "...";

                embed.AddField(header, content, false);
            }
        }

        await ReplyAsync(message, embed.Build());
    }

    private void AppendCommandLine(StringBuilder sb, Models.CommandInfo cmd)
    {
        var permissions = GetPermissionText(cmd);
        sb.AppendLine($"• **{cmd.Name}** — {Truncate(cmd.Description, 80)}");
        if (!string.IsNullOrEmpty(permissions))
            sb.AppendLine($"    _Yêu cầu: {permissions}_");
        sb.AppendLine();
    }

    private string GetPermissionText(Models.CommandInfo cmd)
    {
        var permissions = new List<string>();

        foreach (var guildPerm in cmd.GuildPermissions)
            permissions.Add($"Guild: {guildPerm.Permission}");

        foreach (var channelPerm in cmd.ChannelPermissions)
            permissions.Add($"Channel: {channelPerm.Permission}");

        return string.Join(", ", permissions);
    }

    private bool HasUserPermission(SocketGuildUser? guildUser, Models.CommandInfo cmd)
    {
        if ((cmd.GuildPermissions.Any() || cmd.ChannelPermissions.Any()) && guildUser == null)
            return false;

        foreach (var guildPerm in cmd.GuildPermissions)
        {
            if (guildUser != null && !guildUser.GuildPermissions.Has(guildPerm.Permission))
                return false;
        }

        // Nếu bạn có logic kiểm tra quyền kênh (channel) ở đâu khác, có thể bổ sung ở đây.
        return true;
    }

    private static string Truncate(string s, int max)
        => string.IsNullOrEmpty(s) ? s : (s.Length <= max ? s : s.Substring(0, max - 3) + "...");
}
