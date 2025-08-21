using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using DiscordBot.Modules.Generals;

namespace DiscordBot.Services;
public class HelpComponentHandler : InteractionModuleBase<SocketInteractionContext>
{
    private readonly InteractionService _interactionService;

    public HelpComponentHandler(InteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    [ComponentInteraction("help_refresh")]
    public async Task HandleRefresh()
    {
        // Không tạo instance mới, mà gọi trực tiếp method với context hiện tại
        await UpdateInteractionWithAllCommands();
    }

    [ComponentInteraction("help_back")]
    public async Task HandleBack()
    {
        // Tương tự cho back button
        await UpdateInteractionWithAllCommands();
    }
    private async Task UpdateInteractionWithAllCommands()
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

        await ModifyOriginalResponseAsync(x =>
        {
            x.Embed = embed.Build();
            x.Components = components;
        });
    }
}
