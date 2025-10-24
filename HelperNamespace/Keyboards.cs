using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Models;

namespace HelperNamespce
{
	public static class Keyboards
	{
		public static ReplyKeyboardMarkup MakeMainMenuKeyboard()
		{
			return new ReplyKeyboardMarkup(new[]
			{
				new KeyboardButton[]{ "Анкета👤" },
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

			foreach(var group in groups)
			{
				if(group != null && group.Name != null) 
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

        #region In progres...
        //public static InlineKeyboardMarkup TakeGene()
        //{
        //	return new InlineKeyboardMarkup(new[]
        //	{
        //		new[]
        //		{
        //			InlineKeyboardButton.WithCallbackData("", ""),
        //			InlineKeyboardButton.WithCallbackData("", "")
        //		}
        //	});
        //}
        #endregion
    }


}
