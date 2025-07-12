using HelperNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace ModesLogic
{
	public class ModesHandlers
	{
		#region Mainmenu
		public static async Task MainMenuMode(ITelegramBotClient bot, Update update, CancellationToken clt)
		{
			long? chatID = TelegramBotUtilities.ReturnChatID(update);
			var username = TelegramBotUtilities.ReturnUsername(update);
			Console.WriteLine($"Пользователь {username} запустил бот");
			Message message = await TelegramBotUtilities.DisplayMainMenuKeyboard(bot, chatID, "Выберите действие: ", clt);
		}
		#endregion

		#region Profile
		public static async Task ProfileMode(ITelegramBotClient bot, Update update, CancellationToken clt)
		{
			long? chatID = TelegramBotUtilities.ReturnChatID(update);
			var keyboard = Keyboards.MakeReturnKeyboard();
			string username = TelegramBotUtilities.ReturnUsername(update);
			string messageForButton = TelegramBotUtilities.ReturnProfileText(username);
			Message message = await bot.SendMessage(
				chatId: chatID,
				text: messageForButton,
				replyMarkup: keyboard,
				cancellationToken: clt
				);
		}
		#endregion
	}
}
