using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace MainMenu
{
	public class MainMenuMethods
	{
		#region Mainmenu
		public static async Task MainMenuMode(ITelegramBotClient bot, Update update, CancellationToken clt)
		{
			long chatID = update.Message.Chat.Id;
			var username = HelperNamespace.HelperMethods.ReturnUsername(update);
			Console.WriteLine($"Пользователь {username} запустил бот");
			Message message = await ShowUserKeyboard(bot, chatID, "Выберите действие: ", clt);
		}

		static async Task<Message> ShowUserKeyboard(ITelegramBotClient bot, long ChatID, string TextWithButtons, CancellationToken cancellationToken)
		{
			var keyboard = MakeMainMenuKeyboard();
			Message message = await bot.SendMessage(
				chatId: ChatID,
				text: TextWithButtons,
				replyMarkup: keyboard,
				cancellationToken: cancellationToken
				);
			return message;
		}
		static ReplyKeyboardMarkup MakeMainMenuKeyboard()
		{
			return new ReplyKeyboardMarkup(new[]
			{
				new KeyboardButton[]{ "Профиль👤" },
				new KeyboardButton[]{ "Выбор кандидата🪩" },
				new KeyboardButton[]{ "Убарть себя из списка📌" },
				new KeyboardButton[]{ "Вернуться назад" }
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		#endregion
	}
}
