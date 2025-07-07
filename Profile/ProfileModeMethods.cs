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

namespace Profile
{
	public static class ProfileModeMethods
	{
		public static async Task ProfileMode(ITelegramBotClient bot, Update update, CancellationToken clt)
		{
			long chatID = update.Message.Chat.Id;
			var keyboard = MakeReturnKeyboard();
			string username = HelperMethods.ReturnUsername(update);
			string messageForButton = $"Имя пользователя: {username}\n" +
				$"Группа: \n" +
				$"ФИО профиля: \n" +
				$"Данные профиля можно редактировать👌\n";
			Message message = await bot.SendMessage(
				chatId: chatID,
				text: messageForButton,
				replyMarkup: keyboard,
				cancellationToken: clt
				);
		}

		static ReplyMarkup MakeReturnKeyboard()
		{
			return new ReplyKeyboardMarkup(new[]
			{
			new KeyboardButton[]{ "Вернуться назад" },
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
	}
}
