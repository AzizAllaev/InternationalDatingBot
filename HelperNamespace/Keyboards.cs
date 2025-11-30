using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Models;

namespace HelperNamespce
{
	public class Keyboards
	{
		public static ReplyKeyboardMarkup MakeMainMenuKeyboard()
		{
			return new ReplyKeyboardMarkup(
			[
				["Анкета👤"],
				["Выбор кандидата🪩"],
				["Убарть себя из списка📌"],
				["Назад🔙"]
			])
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup MakeReturnKeyboard()
		{
			return new ReplyKeyboardMarkup(
			[
			["Вернуться назад"],
			])
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup StartRegistrationKeyboard()
		{
			return new ReplyKeyboardMarkup(
			[
				["Данные анкеты👁️"],
			])
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static InlineKeyboardMarkup TakeGenderKeyboard()
		{
			return new InlineKeyboardMarkup(
			[
				[
					InlineKeyboardButton.WithCallbackData("Парень", "Male"),
					InlineKeyboardButton.WithCallbackData("Девушка", "Female")
				]
			]);
		}
		public static InlineKeyboardMarkup TakeGroupKeyboard(List<Group> groups)
		{
			var buttons = new List<List<InlineKeyboardButton>>();

			foreach (var group in groups)
			{
				if (group != null && group.Name != null)
					buttons.Add(
				[
					InlineKeyboardButton.WithCallbackData(group.Name, $"{group.Name}")
				]);
			}
			return new InlineKeyboardMarkup(buttons);
		}
		public static ReplyKeyboardMarkup ReturnFromProfile()
		{
			return new ReplyKeyboardMarkup(
			[
				["Вернуться назад"],
				["Заполнить заново"]
			])
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup ContinueReg()
		{
			return new ReplyKeyboardMarkup(
			[
				["Подтверждаю✅"],
			])
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup ReturnFromReg()
		{
			return new ReplyKeyboardMarkup(
			[
				["Анкета👤"],
				["Заполнить заново"],
				["Вернуться назад"]
			])
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup ChooseModeInPS()
		{
			return new ReplyKeyboardMarkup(
			[
				["Поиск пары🎆"],
				["Кто меня лайкнул⁉️"],
				["Вернуться назад"]
			])
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup LikeDislikeButtons()
		{
			return new ReplyKeyboardMarkup(
			[
				["👍", "👎"],
				["Вернуться назад"]
			])
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup MainOptions()
		{
			return new ReplyKeyboardMarkup(
			[
				["Оставить заявку🪧"],
				["Дополнительные функции"]
			])
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup ConfirmButton()
		{
			return new ReplyKeyboardMarkup(
			[
				["Я прочитал"]
			])
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup ContinueApplicationSend()
		{
			return new ReplyKeyboardMarkup(
			[
				["Подтверждаю☑️"],
			])
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup ContinueOrReturnButton()
		{
			return new ReplyKeyboardMarkup(
			[
				["Подтверждаю☑️"],
				["Заново"]
			])
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
	}
}
