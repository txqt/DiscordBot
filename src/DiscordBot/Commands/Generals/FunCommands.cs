using System.Net.Http;
using Discord;
using Discord.WebSocket;
using DiscordBot.Attributes;
using DiscordBot.Commands;
using Newtonsoft.Json.Linq;

[DiscordCommand("fun", "Các lệnh giải trí")]
public class FunCommands : BaseCommand
{
    private static readonly HttpClient _http = new HttpClient();
    private static readonly Random _rand = new Random();

    public override async Task ExecuteAsync(SocketMessage message, string[] args)
    {
        if (args.Length == 0)
        {
            await ReplyAsync(message, "❌ Dùng: @Bot fun <meme|8ball|roll|ship|joke>");
            return;
        }

        string cmd = args[0].ToLower();

        switch (cmd)
        {
            case "meme":
                await MemeCommand(message);
                break;

            case "8ball":
                await EightBallCommand(message, args.Skip(1).ToArray());
                break;

            case "roll":
                await RollCommand(message, args.Skip(1).ToArray());
                break;

            case "joke":
                await JokeCommand(message);
                break;

            default:
                await ReplyAsync(message, "❌ Không có lệnh này trong nhóm fun.");
                break;
        }
    }

    private async Task MemeCommand(SocketMessage message)
    {
        var json = await _http.GetStringAsync("https://meme-api.com/gimme");
        var obj = JObject.Parse(json);

        string? title = obj["title"]?.ToString();
        string? url = obj["url"]?.ToString();
        string? postLink = obj["postLink"]?.ToString();

        if (title == null || url == null || postLink == null)
        {
            await ReplyAsync(message, "❌ Không thể lấy meme từ API.");
            return;
        }

        var embed = new EmbedBuilder()
            .WithTitle(title)
            .WithUrl(postLink)
            .WithImageUrl(url)
            .WithColor(Color.Green)
            .Build();

        await ReplyAsync(message, embed: embed);
    }

    private async Task EightBallCommand(SocketMessage message, string[] args)
    {
        if (args.Length == 0)
        {
            await ReplyAsync(message, "❌ Bạn cần đặt một câu hỏi cho 8ball!");
            return;
        }

        string[] responses = {
            "Có ✅", "Không ❌", "Có thể 🤔", "Chắc chắn rồi 😎",
            "Không đời nào 😱", "Hỏi lại sau nhé ⏳"
        };

        string answer = responses[_rand.Next(responses.Length)];
        await ReplyAsync(message, $"🎱 {answer}");
    }

    private async Task RollCommand(SocketMessage message, string[] args)
    {
        int max = 6;
        if (args.Length > 0 && int.TryParse(args[0], out int input) && input > 1)
        {
            max = input;
        }

        int result = _rand.Next(1, max + 1);
        await ReplyAsync(message, $"🎲 Bạn tung được: **{result}** (1-{max})");
    }

    private async Task JokeCommand(SocketMessage message)
    {
        var json = await _http.GetStringAsync("https://v2.jokeapi.dev/joke/Any");
        var obj = JObject.Parse(json);

        string? joke;
        if (obj["type"]?.ToString() == "twopart")
        {
            joke = obj["setup"] + "\n" + obj["delivery"];
        }
        else
        {
            joke = obj["joke"]?.ToString();
        }

        await ReplyAsync(message, $"😂 {joke}");
    }
}
