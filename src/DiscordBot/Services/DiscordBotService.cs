﻿using Discord;
using Discord.WebSocket;
using DiscordBot.Models;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Services;

/// <summary>
/// Main bot service that manages lifecycle events, logging, and command handling.
/// </summary>
public class DiscordBotService
{
    private readonly DiscordSocketClient _client;
    private readonly CommandHandlerService _commandHandler;
    private readonly BotConfig _config;
    private readonly ILogger<DiscordBotService> _logger;

    public DiscordBotService(
        DiscordSocketClient client,
        CommandHandlerService commandHandler,
        BotConfig config,
        ILogger<DiscordBotService> logger)
    {
        _client = client;
        _commandHandler = commandHandler;
        _config = config;
        _logger = logger;

        // Subscribe to Discord events
        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.MessageReceived += _commandHandler.HandleMessageAsync;
    }

    /// <summary>
    /// Starts the bot and connects to Discord.
    /// </summary>
    public async Task StartAsync()
    {
        if (string.IsNullOrWhiteSpace(_config.Token))
        {
            _logger.LogError("Bot token is not configured. Please set the token in appsettings.json");
            return;
        }

        await _client.LoginAsync(TokenType.Bot, _config.Token);
        await _client.StartAsync();
    }

    /// <summary>
    /// Stops the bot and logs out from Discord.
    /// </summary>
    public async Task StopAsync()
    {
        await _client.StopAsync();
        await _client.LogoutAsync();
    }

    /// <summary>
    /// Handles log events from Discord.NET and maps them to Microsoft.Extensions.Logging levels.
    /// </summary>
    private Task LogAsync(LogMessage log)
    {
        var logLevel = log.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Trace,
            _ => LogLevel.Information
        };

        _logger.Log(logLevel, log.Exception, "{Source}: {Message}", log.Source, log.Message);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the bot has successfully connected and is ready.
    /// </summary>
    private Task ReadyAsync()
    {
        _logger.LogInformation("Bot {Username} is connected and ready!", _client.CurrentUser.Username);
        return Task.CompletedTask;
    }
}
