using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;

namespace DiscordBot.Commands.Fun;

public class FunModule : InteractionModuleBase<SocketInteractionContext>
{
    private static readonly HttpClient _http = new HttpClient();
    private static readonly Random _rand = new Random();

    // Meme command
    [SlashCommand("meme", "Lấy một meme ngẫu nhiên từ API")]
    public async Task MemeAsync()
    {
        var json = await _http.GetStringAsync("https://meme-api.com/gimme");
        var obj = JObject.Parse(json);

        string? title = obj["title"]?.ToString();
        string? url = obj["url"]?.ToString();
        string? postLink = obj["postLink"]?.ToString();

        if (title == null || url == null || postLink == null)
        {
            await RespondAsync("❌ Không thể lấy meme từ API.");
            return;
        }

        var embed = new EmbedBuilder()
            .WithTitle(title)
            .WithUrl(postLink)
            .WithImageUrl(url)
            .WithColor(Color.Green)
            .Build();

        await RespondAsync(embed: embed);
    }

    // 8ball command
    [SlashCommand("8ball", "Hỏi quả cầu tiên tri 🎱")]
    public async Task EightBallAsync([Summary("câu_hỏi", "Nhập câu hỏi cho 8ball")] string question)
    {
        string[] responses = {
            "Có ✅", "Không ❌", "Có thể 🤔", "Chắc chắn rồi 😎",
            "Không đời nào 😱", "Hỏi lại sau nhé ⏳"
        };

        string answer = responses[_rand.Next(responses.Length)];
        await RespondAsync($"🎱 {answer}");
    }

    // Roll command
    [SlashCommand("roll", "Tung xúc xắc (mặc định 1-6)")]
    public async Task RollAsync([Summary("max", "Giá trị tối đa (mặc định 6)")] int max = 6)
    {
        if (max < 2) max = 6; // tránh invalid input

        int result = _rand.Next(1, max + 1);
        await RespondAsync($"🎲 Bạn tung được: **{result}** (1-{max})");
    }

    // Joke command
    [SlashCommand("joke", "Kể một câu chuyện cười ngẫu nhiên")]
    public async Task JokeAsync()
    {
        var json = await _http.GetStringAsync("https://v2.jokeapi.dev/joke/Any");
        var obj = JObject.Parse(json);

        string? joke;
        if (obj["type"]?.ToString() == "twopart")
        {
            joke = $"{obj["setup"]}\n{obj["delivery"]}";
        }
        else
        {
            joke = obj["joke"]?.ToString();
        }

        await RespondAsync($"😂 {joke}");
    }

    [SlashCommand("say", "Bot sẽ nói lại nội dung bạn nhập")]
    public async Task SayCommand(
    [Summary("text", "Nội dung bot sẽ gửi")] string text)
    {
        await RespondAsync(text);
    }

}
