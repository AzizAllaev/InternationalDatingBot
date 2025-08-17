using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace HelperNamespace
{
	public static class TelegramBotUtilities
	{
		public static string ReturnProfileText(string username)
		{
			string text = $"Имя пользователя: {username}\n" +
				$"Группа: \n" +
				$"ФИО профиля: \n" +
				$"Данные профиля можно редактировать👌\n";
			return text;
		}
		public static async Task<Message> DisplayMainMenuKeyboard(ITelegramBotClient bot, long? ChatID, string TextWithButtons, CancellationToken cancellationToken)
		{
			var keyboard = Keyboards.MakeMainMenuKeyboard();
			Message message = await bot.SendMessage(
				chatId: ChatID,
				text: TextWithButtons,
				replyMarkup: keyboard,
				cancellationToken: cancellationToken
				);
			return message;
		}
		public static long? ReturnChatID(Update update)
		{
			if(update.Message != null) 
				return update.Message.Chat.Id;
			if (update.CallbackQuery != null)
				if(update.CallbackQuery.Message != null)
					return update.CallbackQuery.Message.Chat.Id;
			if(update.EditedMessage != null)
				return update.EditedMessage.Chat.Id;

			return null;
		}
		public static string? ReturnNewMessage(Update update)
		{
			if (update.Message != null)
				if(update.Message.Text != null)
					return update.Message.Text;
	
			return null;
		}
		public static string ReturnUsername(Update update)
		{
			var user = update.Message.From;
			string username = user.Username;
			return username;
		}
	}

	public static class Keyboards
	{
		public static ReplyKeyboardMarkup MakeMainMenuKeyboard()
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
		public static ReplyMarkup MakeReturnKeyboard()
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
