using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot.Modules.Moderations;

public class ClearModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("clear", "Xóa tin nhắn trong kênh")]
    [RequireUserPermission(GuildPermission.ManageMessages)]
    [RequireBotPermission(GuildPermission.ManageMessages)]
    public async Task ClearAsync(
            [Summary("amount", "Số lượng tin nhắn cần xóa (1-100)")]
            [MinValue(1)]
            [MaxValue(100)]
            int amount)
    {
        // Defer response vì xóa tin nhắn có thể mất thời gian
        await DeferAsync(ephemeral: true);

        try
        {
            var channel = Context.Channel as ITextChannel;
            if (channel == null)
            {
                await FollowupAsync("❌ Lệnh này chỉ có thể sử dụng trong text channel!", ephemeral: true);
                return;
            }

            // Lấy tin nhắn (không bao gồm slash command message)
            var messages = await channel.GetMessagesAsync(amount, CacheMode.AllowDownload).FlattenAsync();

            if (!messages.Any())
            {
                await FollowupAsync("❌ Không tìm thấy tin nhắn nào để xóa!", ephemeral: true);
                return;
            }

            // Lọc tin nhắn có thể xóa được
            var deletableMessages = new List<IMessage>();
            var now = DateTimeOffset.UtcNow;

            foreach (var message in messages)
            {
                // Kiểm tra tin nhắn có thể xóa không (dưới 14 ngày và không phải system message)
                if ((now - message.Timestamp).TotalDays < 14 &&
                    message.Type == MessageType.Default ||
                    message.Type == MessageType.Reply)
                {
                    deletableMessages.Add(message);
                }
            }

            if (!deletableMessages.Any())
            {
                await FollowupAsync("❌ Không có tin nhắn nào có thể xóa! (Tin nhắn cũ hơn 14 ngày hoặc tin nhắn hệ thống không thể xóa)", ephemeral: true);
                return;
            }

            int successfulDeletes = 0;
            int failedDeletes = 0;

            // Xóa từng tin nhắn một cách an toàn
            if (deletableMessages.Count == 1)
            {
                try
                {
                    await deletableMessages[0].DeleteAsync();
                    successfulDeletes = 1;
                }
                catch (Discord.Net.HttpException ex) when (ex.DiscordCode == DiscordErrorCode.UnknownMessage)
                {
                    failedDeletes = 1;
                }
            }
            else
            {
                // Chia nhỏ thành các batch để tránh rate limit
                var batches = deletableMessages.Chunk(100); // Discord giới hạn 100 tin nhắn/lần

                foreach (var batch in batches)
                {
                    try
                    {
                        // Kiểm tra lại tin nhắn có tồn tại không trước khi xóa
                        var validMessages = new List<IMessage>();
                        foreach (var msg in batch)
                        {
                            try
                            {
                                // Thử get tin nhắn để kiểm tra còn tồn tại không
                                var checkMsg = await channel.GetMessageAsync(msg.Id);
                                if (checkMsg != null)
                                {
                                    validMessages.Add(msg);
                                }
                            }
                            catch
                            {
                                failedDeletes++;
                            }
                        }

                        if (validMessages.Count > 1)
                        {
                            await channel.DeleteMessagesAsync(validMessages);
                            successfulDeletes += validMessages.Count;
                        }
                        else if (validMessages.Count == 1)
                        {
                            await validMessages[0].DeleteAsync();
                            successfulDeletes += 1;
                        }

                        // Delay ngắn để tránh rate limit
                        if (batches.Count() > 1)
                            await Task.Delay(1000);
                    }
                    catch (Discord.Net.HttpException ex) when (ex.DiscordCode == DiscordErrorCode.UnknownMessage)
                    {
                        failedDeletes += batch.Count();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi xóa batch: {ex.Message}");
                        failedDeletes += batch.Count();
                    }
                }
            }

            // Tạo response message
            var responseMessage = $"✅ Đã xóa {successfulDeletes} tin nhắn!";
            if (failedDeletes > 0)
            {
                responseMessage += $"\n⚠️ Không thể xóa {failedDeletes} tin nhắn (có thể đã bị xóa hoặc là tin nhắn hệ thống).";
            }

            await FollowupAsync(responseMessage, ephemeral: true);
        }
        catch (Discord.Net.HttpException ex) when (ex.DiscordCode == DiscordErrorCode.MissingPermissions)
        {
            await FollowupAsync("❌ Bot không có quyền xóa tin nhắn trong kênh này!", ephemeral: true);
        }
        catch (Discord.Net.HttpException ex) when (ex.DiscordCode == DiscordErrorCode.UnknownMessage)
        {
            await FollowupAsync("❌ Một số tin nhắn không tồn tại hoặc đã bị xóa!", ephemeral: true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi trong ClearAsync: {ex}");
            await FollowupAsync($"❌ Có lỗi không mong muốn: {ex.Message}", ephemeral: true);
        }
    }
}
