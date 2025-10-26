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
				string CallBackqueryData = update.CallbackQuery.Data;
				using var db = new AppDbContext();
				UserProfile? user = db.Users.FirstOrDefault(u => u.TelegramID == update.CallbackQuery.From.Id);
				var userregStat = db.RegistrationStatuses.FirstOrDefault(ureg => ureg.ProfileId == update.CallbackQuery.From.Id);
				if (CallBackqueryData != null && user != null && userregStat != null && update.CallbackQuery.Message != null)
				{
					if(CallBackqueryData == "Male" || CallBackqueryData == "Female" && userregStat.UserRegStatus == 1)
					{
						await ModesHandlers.AnswerOnTakeGender(CallBackqueryData, user, update, bot, db, userregStat);
					}
					else if(db.Groups.Any(grp => grp.Name == CallBackqueryData && userregStat.UserRegStatus == 2))
					{
						await ModesHandlers.AnswerOnTakeGroup(db, CallBackqueryData, update, user, bot, userregStat);
					}
				}
			}
			if(update.Message != null && update.Message.From != null)
			{
				using var db = new AppDbContext();
				var userregStat = db.RegistrationStatuses.FirstOrDefault(ureg => ureg.ProfileId == update.Message.From.Id);
				var user = db.Users.FirstOrDefault(u => u.TelegramID == update.Message.From.Id);
				if (userregStat != null)
				{
					if (update.Message.Text != null && update.Message.Text != "Подтверждаю✅")
					{
						if (userregStat.UserRegStatus == 3)
						{
							if (user != null)
							{
								await ModesHandlers.AnswerOnTakeName(bot, update.Message.Text, update, user, db, userregStat);
							}
						}
						else if (userregStat.UserRegStatus == 4)
						{
							if(user != null)
							{
								await ModesHandlers.AnswerOnTakeLastName(bot, update.Message.Text, update, user, db, userregStat);
							}
						}
					}
					db.SaveChanges();
				}
			}


			if (update.Message != null)
			{
				string? text = TelegramBotUtilities.ReturnNewMessage(update);
				if (text != null)
				{
					using AppDbContext db = new AppDbContext();
					switch (text)
					{
						case "Подтверждаю✅":
							await ModesHandlers.TakeData(bot, update, clt, db);
							break;
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
