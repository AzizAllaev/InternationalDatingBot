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
			return new ReplyKeyboardMarkup(new[]
			{
				new KeyboardButton[]{ "Анкета👤" },
				new KeyboardButton[]{ "Выбор кандидата🪩" },
				new KeyboardButton[]{ "Убарть себя из списка📌" },
				new KeyboardButton[] { "Назад🔙" }
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
				new KeyboardButton[]{ "Данные анкеты👁️" },
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static InlineKeyboardMarkup TakeGenderKeyboard()
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
		public static InlineKeyboardMarkup TakeGroupKeyboard(List<Group> groups)
		{
			var buttons = new List<List<InlineKeyboardButton>>();

			foreach (var group in groups)
			{
				if (group != null && group.Name != null)
					buttons.Add(new List<InlineKeyboardButton>
				{
					InlineKeyboardButton.WithCallbackData(group.Name, $"{group.Name}")
				});
			}
			return new InlineKeyboardMarkup(buttons);
		}
		public static ReplyKeyboardMarkup ReturnFromProfile()
		{
			return new ReplyKeyboardMarkup(new[]
			{
				new KeyboardButton[]{ "Вернуться назад" },
				new KeyboardButton[]{ "Заполнить заново" }
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup ContinueReg()
		{
			return new ReplyKeyboardMarkup(new[]
			{
				new KeyboardButton[]{ "Подтверждаю✅" },
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup ReturnFromReg()
		{
			return new ReplyKeyboardMarkup(new[]
			{
				new KeyboardButton[]{ "Анкета👤" },
				new KeyboardButton[]{ "Заполнить заново" },
				new KeyboardButton[]{ "Вернуться назад" }
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup ChooseModeInPS()
		{
			return new ReplyKeyboardMarkup(new[]
			{
				new KeyboardButton[]{ "Поиск пары🎆" },
				new KeyboardButton[]{ "Кто меня лайкнул⁉️" },
				new KeyboardButton[]{ "Вернуться назад" }
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup LikeDislikeButtons()
		{
			return new ReplyKeyboardMarkup(new[]
			{
				new KeyboardButton[]{ "👍", "👎" },
				new KeyboardButton[]{ "Вернуться назад"}
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup MainOptions()
		{
			return new ReplyKeyboardMarkup(new[]
			{
				new KeyboardButton[]{ "Оставить заявку🪧" },
				new KeyboardButton[]{ "Дополнительные функции" }
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup ConfirmButton()
		{
			return new ReplyKeyboardMarkup(new[]
			{
				new KeyboardButton[]{ "Я прочитал" }
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
		public static ReplyKeyboardMarkup ContinueApplicationSend()
		{
			return new ReplyKeyboardMarkup(new[]
			{
				new KeyboardButton[]{ "Подтверждаю☑️" },
			})
			{
				ResizeKeyboard = true,
				OneTimeKeyboard = false
			};
		}
	}
}
