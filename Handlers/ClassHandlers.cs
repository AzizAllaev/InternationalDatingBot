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
				await ModesLogic.UpdateTypesHandler.WhenCallBackquery(bot, update);
			}
			if(update.Message != null && update.Message.From != null)
			{
				await ModesLogic.UpdateTypesHandler.WhenMessageForProfile(bot, update);
			}
			if(update.Message != null && update.Message.Photo != null)
			{
				await ModesLogic.UpdateTypesHandler.WhenPhotoForProfile(bot, update);
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
							if (!ModesLogic.ModesHandlers.CheckStatus(update, db))
							{
								await ModesHandlers.StartUserRegistration(bot, update, clt, db);
							}
							else
							{
								await ModesHandlers.MainMenuMode(bot, update, clt);
							}
							break;
						case "Анкета👤":
							await ModesHandlers.ProfileMode(bot, update, clt, db);
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
			await Task.Run(() =>
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"Ошибка: {exception.Message}");
				Console.WriteLine($"Источник: {source}");
				Console.ResetColor();
			});
		}


	}
}
