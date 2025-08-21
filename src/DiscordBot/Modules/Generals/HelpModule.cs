using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot.Modules.Generals;

public class HelpModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly InteractionService _interactionService;

    public HelpModule(InteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    [SlashCommand("help", "Hiển thị danh sách các lệnh có sẵn")]
    public async Task HelpCommand(
        [Summary("command", "Tên lệnh cụ thể để xem chi tiết")] string? commandName = null)
    {
        try
        {
            if (string.IsNullOrEmpty(commandName))
            {
                // Hiển thị tất cả commands
                await ShowAllCommands();
            }
            else
            {
                // Hiển thị chi tiết của một command cụ thể
                await ShowSpecificCommand(commandName);
            }
        }
        catch (Exception ex)
        {
            await RespondAsync($"❌ Có lỗi xảy ra: {ex.Message}", ephemeral: true);
        }
    }

    private async Task ShowAllCommands()
    {
        var embed = new EmbedBuilder()
            .WithTitle("📋 Danh sách lệnh")
            .WithDescription("Dưới đây là tất cả các lệnh có sẵn:")
            .WithColor(Color.Blue)
            .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
            .WithTimestamp(DateTimeOffset.Now)
            .WithFooter("Sử dụng /help <tên lệnh> để xem chi tiết");

        var modules = _interactionService.Modules;
        var commandGroups = new Dictionary<string, List<string>>();

        foreach (var module in modules)
        {
            var moduleName = module.Name.Replace("Module", "") ?? "Chung";

            if (!commandGroups.ContainsKey(moduleName))
                commandGroups[moduleName] = new List<string>();

            foreach (var command in module.SlashCommands)
            {
                var commandInfo = $"`/{command.Name}` - {command.Description ?? "Không có mô tả"}";
                commandGroups[moduleName].Add(commandInfo);
            }
        }

        foreach (var group in commandGroups)
        {
            if (group.Value.Any())
            {
                var commandList = string.Join("\n", group.Value);
                embed.AddField($"📁 {group.Key}", commandList, false);
            }
        }

        if (!commandGroups.Any() || !commandGroups.SelectMany(g => g.Value).Any())
        {
            embed.AddField("❌ Không có lệnh nào", "Hiện tại không có lệnh nào được đăng ký.", false);
        }

        var components = new ComponentBuilder()
            .WithButton("🔄 Refresh", "help_refresh", ButtonStyle.Secondary)
            .WithButton("ℹ️ Bot Info", "bot_info", ButtonStyle.Primary)
            .Build();

        await RespondAsync(embed: embed.Build(), components: components);
    }

    private async Task ShowSpecificCommand(string commandName)
    {
        var command = _interactionService.SlashCommands
            .FirstOrDefault(c => c.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));

        if (command == null)
        {
            var embed = new EmbedBuilder()
                .WithTitle("❌ Không tìm thấy lệnh")
                .WithDescription($"Không tìm thấy lệnh `{commandName}`")
                .WithColor(Color.Red)
                .WithTimestamp(DateTimeOffset.Now);

            await RespondAsync(embed: embed.Build(), ephemeral: true);
            return;
        }

        var detailEmbed = new EmbedBuilder()
            .WithTitle($"📖 Chi tiết lệnh: /{command.Name}")
            .WithDescription(command.Description ?? "Không có mô tả")
            .WithColor(Color.Green)
            .WithTimestamp(DateTimeOffset.Now);

        // Thêm thông tin về parameters
        if (command.Parameters.Any())
        {
            var parameters = command.Parameters.Select(p =>
            {
                var required = p.IsRequired ? "**Bắt buộc**" : "*Tùy chọn*";
                var defaultValue = p.DefaultValue != null ? $" (Mặc định: `{p.DefaultValue}`)" : "";
                return $"• `{p.Name}` ({p.GetType().Name}) - {required}{defaultValue}\n  └ {p.Description ?? "Không có mô tả"}";
            });

            detailEmbed.AddField("🔧 Tham số", string.Join("\n\n", parameters), false);
        }
        else
        {
            detailEmbed.AddField("🔧 Tham số", "Lệnh này không có tham số", false);
        }

        // Thêm ví dụ sử dụng
        var example = $"/{command.Name}";
        if (command.Parameters.Any())
        {
            var exampleParams = command.Parameters.Take(2).Select(p =>
                p.IsRequired ? $"{p.Name}:value" : $"[{p.Name}:value]");
            example += " " + string.Join(" ", exampleParams);
        }

        detailEmbed.AddField("💡 Ví dụ sử dụng", $"`{example}`", false);

        var backButton = new ComponentBuilder()
            .WithButton("⬅️ Quay lại", "help_back", ButtonStyle.Secondary)
            .Build();

        await RespondAsync(embed: detailEmbed.Build(), components: backButton);
    }
}
