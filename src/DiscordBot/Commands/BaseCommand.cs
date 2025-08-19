using Discord;
using Discord.WebSocket;

namespace DiscordBot.Commands;

/// <summary>
/// Base class for all Discord commands. 
/// Provides a common execution contract and helper reply methods.
/// </summary>
public abstract class BaseCommand
{
    /// <summary>
    /// Executes the command with the given message and arguments.
    /// </summary>
    public abstract Task ExecuteAsync(SocketMessage message, string[] args);

    /// <summary>
    /// Sends a reply to the message author with optional text, TTS, and embed.
    /// </summary>
    protected async Task ReplyAsync(SocketMessage message, string content, bool isTTS = false, Embed? embed = null)
    {
        if (message.Channel is IMessageChannel channel)
        {
            var replyContent = $"{message.Author.Mention} {content}".Trim();
            await channel.SendMessageAsync(replyContent, isTTS, embed);
        }
    }

    /// <summary>
    /// Sends a reply with only an embed.
    /// </summary>
    protected async Task ReplyAsync(SocketMessage message, Embed embed)
    {
        await ReplyAsync(message, string.Empty, false, embed);
    }
}
