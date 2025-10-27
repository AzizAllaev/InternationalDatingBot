using HelperNamespce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;
using Models;

namespace HelperNamespace
{
	public static class TelegramBotUtilities
	{
		#region Text
		public static string? ReturnProfileText(AppDbContext db, Update update)
		{
			if (update.Message != null && update.Message.From != null)
			{
				var user = db.Users.FirstOrDefault(u => u.TelegramID == update.Message.From.Id);
				if (user != null)
				{
					var group = db.Groups.FirstOrDefault(g => g.Id == user.GroupID);
					if (group != null)
					{
						string text = $"Имя пользователя: {user.Username}\n" +
						$"Группа: {group.Name}\n" +
						$"ФИ профиля: {user.Name} {user.LastName}\n";
						return text;
					}
					else
					{
						string text = "Ваш заполнень не полностью👤";
						return text;
					}
				}
			}
			else if(update.CallbackQuery != null && update.CallbackQuery.From != null)
			{
				var user = db.Users.FirstOrDefault(u => u.TelegramID == update.CallbackQuery.From.Id);
				if (user != null)
				{
					var group = db.Groups.FirstOrDefault(g => g.Id == user.GroupID);
					if (group != null)
					{
						string text = $"Имя пользователя: {user.Username}\n" +
						$"Группа: {group.Name}\n" +
						$"ФИ профиля: {user.Name} {user.LastName}\n";
						return text;
					}
					else
					{
						string text = "Ваш заполнень не полностью👤";
						return text;
					}
				}
			}
			else
			{
				Console.WriteLine("NullReferenceEx, source: TelegramBotUtilities.ReturnProfileText");
			}
			return null;
		}
		public static string StartRegirstrationText()
		{
			string text = "Начать заполнение профиля";
			return text;
		}
		public static string AskGroupText()
		{
			string text = "Выберите вашу группу: ";
			return text;
		}
		#endregion

		#region User handler methods
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
			return update?.Message?.Chat?.Id;
		}
		public static string? ReturnNewMessage(Update update)
		{
			if (update.Message != null)
				if(update.Message.Text != null)
					return update.Message.Text;
	
			return null;
		}
		public static string? ReturnUsername(Update update)
		{
			return update?.Message?.From?.Username;
		}
		#endregion
	}
}
