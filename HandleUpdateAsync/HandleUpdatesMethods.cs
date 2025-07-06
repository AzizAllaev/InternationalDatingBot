using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;


namespace HandleUpdate
{
	public static class HandleUpdatesMethods
	{
		public static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken clt)
		{
			string text = HelperNamespace.UpdateHandlerMethods.ReturnNewMessage(update);
			switch (text)
			{
				case "/start":
					await MainMenu(bot, update, clt);
					break;
				case "Профиль👤":
					break;
				case "Выбор кандидата🪩":
					break;
				case "Убарть себя из списка📌":
					break;
			}
		}

		#region Mainmenu
		static async Task MainMenu(ITelegramBotClient bot, Update update, CancellationToken clt)
		{
			long chatID = update.Message.Chat.Id;
			var username = HelperNamespace.UpdateHandlerMethods.ReturnUsername(update);
			Console.WriteLine($"Пользователь {username} запустил бот");
			Message message = await ShowUserKeyboard(bot, chatID, "Выберите действие: ", clt);
		}

		public static async Task<Message> ShowUserKeyboard(ITelegramBotClient bot, long ChatID, string TextWithButtons, CancellationToken cancellationToken)
		{
			var keyboard = MakeKeyboard();
			Message message = await bot.SendMessage(
				chatId: ChatID,
				text: TextWithButtons,
				replyMarkup: keyboard,
				cancellationToken: cancellationToken
				);
			return message;
		}
		public static ReplyKeyboardMarkup MakeKeyboard()
		{
			return new ReplyKeyboardMarkup(new[]
			{
				new KeyboardButton[]{ "Профиль👤" },
				new KeyboardButton[]{ "Выбор кандидата🪩" },
				new KeyboardButton[]{ "Убарть себя из списка📌" }
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		#endregion
	}
}
