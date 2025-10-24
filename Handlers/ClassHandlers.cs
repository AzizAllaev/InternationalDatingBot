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
			//Cause if user pressed inlinemarkup button
			if (update.CallbackQuery != null && update.CallbackQuery.Data != null)
			{
				await bot.AnswerCallbackQuery(update.CallbackQuery.Id);
				string data = update.CallbackQuery.Data;
				using var db = new AppDbContext();
				var user = db.Users.FirstOrDefault(u => u.TelegramID == update.CallbackQuery.From.Id);
				var userregStat = db.RegistrationStatuses.FirstOrDefault(ureg => ureg.ProfileId == update.CallbackQuery.From.Id);
				if (data != null && user != null && userregStat != null && update.CallbackQuery.Message != null)
				{
					if(data == "Male" || data == "Female")
					{
						user.Gender = data;
						var chatId = update.CallbackQuery.Message.Chat.Id;
						var messageId = update.CallbackQuery.Message.MessageId;
						await bot.DeleteMessage(chatId, messageId);
						userregStat.UserRegStatus = 2;
						await ModesHandlers.TakeData(bot, update, clt, db);
						db.SaveChanges();
					}
					else if(db.Groups.Any(grp => grp.Name == data))
					{
						var group = db.Groups.FirstOrDefault(grp => grp.Name == data);
						var messageId = update.CallbackQuery.Message.MessageId;
						var chatId = update.CallbackQuery.Message.Chat.Id;
						if (group != null)
						{
							await bot.DeleteMessage(chatId, messageId);
							user.group = group;
							user.GroupID = group.Id;
							userregStat.UserRegStatus = 3;
							await ModesHandlers.TakeData(bot, update, clt, db);
							db.SaveChanges();
						}
					}
				}
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
						case "Заполнить заново":
							await ModesHandlers.StartUserRegistration(bot, update, clt, db);
							break;
						case "Данные анкеты👁️":
							await ModesHandlers.TakeData(bot, update, clt, db);
							break;
						case "/start":
							await ModesHandlers.MainMenuMode(bot, update, clt);
							if (!ModesLogic.ModesHandlers.CheckStatus(update, db))
							{
								await ModesHandlers.StartUserRegistration(bot, update, clt, db);
							}
							break;
						case "Анкета👤":
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
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"Ошибка: {exception.Message}");
			Console.WriteLine($"Источник: {source}");
			Console.ResetColor();
		}


	}
}
