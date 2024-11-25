﻿using GrammarNazi.Core.Utilities;
using GrammarNazi.Domain.BotCommands;
using GrammarNazi.Domain.Constants;
using GrammarNazi.Domain.Services;
using GrammarNazi.Domain.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace GrammarNazi.Core.BotCommands.Telegram;

public class RemoveWhiteListCommand : BaseTelegramCommand, ITelegramBotCommand
{
    private readonly IChatConfigurationService _chatConfigurationService;

    public string Command => TelegramBotCommands.RemoveWhiteList;

    public RemoveWhiteListCommand(IChatConfigurationService chatConfigurationService,
        ITelegramBotClientWrapper telegramBotClient)
        : base(telegramBotClient)
    {
        _chatConfigurationService = chatConfigurationService;
    }

    public async Task Handle(Message message)
    {
        await SendTypingNotification(message);

        if (!await IsUserAdmin(message))
        {
            await Client.SendTextMessageAsync(message.Chat.Id, "Only admins can use this command.", replyParameters: message.MessageId);
            return;
        }

        var parameters = message.Text.Split(" ");

        if (parameters.Length == 1)
        {
            await Client.SendTextMessageAsync(message.Chat.Id, $"Parameter not received. Type {TelegramBotCommands.RemoveWhiteList} <word> to remove a Whitelist word.");
        }
        else
        {
            var chatConfig = await _chatConfigurationService.GetConfigurationByChatId(message.Chat.Id);

            var word = parameters[1].Trim();

            if (!chatConfig.WhiteListWords.Contains(word, new CaseInsensitiveEqualityComparer()))
            {
                await Client.SendTextMessageAsync(message.Chat.Id, $"The word '{word}' is not in the WhiteList.");
                return;
            }

            chatConfig.WhiteListWords.RemoveAll(v => v.Equals(word, StringComparison.OrdinalIgnoreCase));

            await _chatConfigurationService.Update(chatConfig);

            await Client.SendTextMessageAsync(message.Chat.Id, $"Word '{word}' removed from the WhiteList.");
        }

        await NotifyIfBotIsNotAdmin(message);
    }
}