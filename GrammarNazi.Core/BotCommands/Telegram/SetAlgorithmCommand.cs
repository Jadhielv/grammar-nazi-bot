﻿using GrammarNazi.Core.Extensions;
using GrammarNazi.Domain.BotCommands;
using GrammarNazi.Domain.Constants;
using GrammarNazi.Domain.Enums;
using GrammarNazi.Domain.Services;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GrammarNazi.Core.BotCommands.Telegram
{
    public class SetAlgorithmCommand : BaseTelegramCommand, ITelegramBotCommand
    {
        private readonly IChatConfigurationService _chatConfigurationService;
        private readonly ITelegramBotClient _client;
        public string Command => TelegramBotCommands.SetAlgorithm;

        public SetAlgorithmCommand(IChatConfigurationService chatConfigurationService,
            ITelegramBotClient telegramBotClient)
            : base(telegramBotClient)
        {
            _chatConfigurationService = chatConfigurationService;
            _client = telegramBotClient;
        }

        public async Task Handle(Message message)
        {
            var messageBuilder = new StringBuilder();

            if (!await IsUserAdmin(message))
            {
                messageBuilder.AppendLine("Only admins can use this command.");
                await _client.SendTextMessageAsync(message.Chat.Id, messageBuilder.ToString(), replyToMessageId: message.MessageId);
                return;
            }

            var parameters = message.Text.Split(" ");
            if (parameters.Length == 1)
            {
                await ShowOptions<GrammarAlgorithms>(message, "Choose Algorithm");
            }
            else
            {
                bool parsedOk = int.TryParse(parameters[1], out int algorithm);

                if (parsedOk && algorithm.IsAssignableToEnum<GrammarAlgorithms>())
                {
                    var chatConfig = await _chatConfigurationService.GetConfigurationByChatId(message.Chat.Id);
                    chatConfig.GrammarAlgorithm = (GrammarAlgorithms)algorithm;

                    await _chatConfigurationService.Update(chatConfig);

                    await _client.SendTextMessageAsync(message.Chat.Id, "Algorithm updated.");
                }
                else
                {
                    await _client.SendTextMessageAsync(message.Chat.Id, $"Invalid parameter. Type {TelegramBotCommands.SetAlgorithm} <algorithm_numer> to set an algorithm.");
                }
            }

            await NotifyIfBotIsNotAdmin(message);
        }
    }
}
