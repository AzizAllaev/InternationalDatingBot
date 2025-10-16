using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace HelperNamespce
{
	public static class Keyboards
	{
		public static ReplyKeyboardMarkup MakeMainMenuKeyboard()
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
		public static ReplyKeyboardMarkup MakeReturnKeyboard()
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
		public static ReplyKeyboardMarkup StartRegistrationKeyboard()
		{
			return new ReplyKeyboardMarkup(new[]
			{
				new KeyboardButton[]{ "Начать заполнение профиля👁️" },
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup TakeGenderKeyboard()
		{
			return new ReplyKeyboardMarkup(new[]
			{
				new KeyboardButton[]{ "Парень" },
				new KeyboardButton[]{ "Девушка" }
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static InlineKeyboardMarkup TakeGroupKeyboard()
		{
			return new InlineKeyboardMarkup(new[]
			{
				new[]
				{
					InlineKeyboardButton.WithCallbackData("Парень", "Male"),
					InlineKeyboardButton.WithCallbackData("Девушка", "Female")
				}
			});
		}
		public static InlineKeyboardMarkup TakeGroupKeyboard()
		{
			return new InlineKeyboardMarkup(new[]
			{
				new[]
				{
					InlineKeyboardButton.WithCallbackData("Парень", "Male"),
					InlineKeyboardButton.WithCallbackData("Девушка", "Female")
				}
			});
		}
	}
}
