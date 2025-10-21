using HelperNamespace;
using ModesLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Models;



namespace Handlers
{
	public class ClassHandlers
	{
		public static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken clt)
		{
			// Cause if user pressed inlinemarkup button
			if (update.CallbackQuery != null)
			{
				using AppDbContext db = new AppDbContext();
				string data = update.CallbackQuery.Data;

				if (db.Groups.Any(group => group.Name == data))
				{
					var user = db.Users.FirstOrDefault(u => u.ProfileId == update.CallbackQuery.From.Id);
					var group = db.Groups.FirstOrDefault(gr => gr.Name == data);
					user.Group = group;
					user.GroupID = group.Id;
				}
				await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, $"Вы выбрали группу: {group.Name}");
				db.SaveChanges();
			}


			// Cause if user pressed replymarkup button
			if (update.Message != null)
			{
				string? text = TelegramBotUtilities.ReturnNewMessage(update);
				if (text != null)
				{
					using AppDbContext db = new AppDbContext();
					switch (text)
					{
						case "Начать заполнение профиля👁️":
							await TakeData(bot, update, cts, db);
							break;
						case "/start":
							await ModesHandlers.MainMenuMode(bot, update, clt);
							if (ModesLogic.ModesHandlers.CheckStatus(update, db))
							{
								await ModesHandlers.StartUserRegistration(bot, update, clt);
							}
							break;
						case "Профиль👤":
							await ModesHandlers.ProfileMode(bot, update, clt);
							break;
						case "Выбор кандидата🪩":
							break;
						case "Убарть себя из списка📌":
							break;
						case "Вернуться назад":
							await ModesHandlers.MainMenuMode(bot, update, clt);
							break;
					}
				}
			}
		}

		
		public static async Task HandleError(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken token)
		{
			Console.WriteLine("Ошибка кода");
			throw new Exception("Фатальная ошибка, бот временно прекращает работу");
		}

	}
}
