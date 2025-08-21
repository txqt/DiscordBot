using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;

namespace DiscordBot.Modules.Generals;
public class RemindModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("remind", "Nhắc nhở bạn sau 1 thời gian nhất định")]
    public async Task Remind(
        [Summary("time", "Thời gian (ví dụ: 10s, 5m, 2h)")] string time,
        [Summary("message", "Nội dung nhắc nhở")] string message)
    {
        await DeferAsync(ephemeral: true); // để tránh timeout
        TimeSpan delay;

        try
        {
            delay = ParseTime(time);
        }
        catch
        {
            await FollowupAsync("Định dạng thời gian không hợp lệ. Ví dụ: 10s, 5m, 2h");
            return;
        }

        // tạo task delay
        _ = Task.Run(async () =>
        {
            await Task.Delay(delay);
            await Context.User.SendMessageAsync($"⏰ Nhắc nhở: {message}");
        });

        await FollowupAsync($"✅ Mình sẽ nhắc bạn sau {time}");
    }

    // Hàm parse thời gian đơn giản
    private TimeSpan ParseTime(string input)
    {
        char unit = input[^1]; // ký tự cuối
        int value = int.Parse(input[..^1]);

        return unit switch
        {
            's' => TimeSpan.FromSeconds(value),
            'm' => TimeSpan.FromMinutes(value),
            'h' => TimeSpan.FromHours(value),
            _ => throw new Exception("Định dạng không hợp lệ")
        };
    }
}
