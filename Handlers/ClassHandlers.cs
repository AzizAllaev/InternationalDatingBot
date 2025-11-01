﻿using HelperNamespace;
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
			using AppDbContext db = new AppDbContext();
			//Case if user press buttons or answer to messages from bot
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
					switch (text)
					{
						// Main buttons of modes
						case "/start":
							if (!await ModesHandlers.CheckStatus(update, db))
							{
								await ModesHandlers.StartUserRegistration(bot, update, clt, db);
							}
							else if (await ModesHandlers.CheckStatus(update, db))
							{
								await ModesHandlers.MainMenuMode(bot, update, clt);
							}
							else
							{
								throw new Exception("After /start user is not found");
							}
							break;
						case "Анкета👤":
							await ModesHandlers.ProfileMode(bot, update, clt, db);
							await ModesHandlers.ChangeModeStatus(update, db, 1);
							break; // !!! Field that send to user UserProfile !!!
						case "Убарть себя из списка📌":
							break; // !!! Field that delete all data about user from DB !!!
						case "Выбор кандидата🪩":
							break; // !!! Field that start partner showcase !!!


						// Service buttons
						case "Данные анкеты👁️":
							await ModesHandlers.TakeData(bot, update, clt, db);// <<--- This methods start registration
							await ModesHandlers.ChangeModeStatus(update, db, 1);
							break; // <<-- Start of user profile registration
						case "Подтверждаю✅":
							await ModesHandlers.TakeData(bot, update, clt, db);
							break; // <<-- Confirmation of profile registration data that fill user
						case "Вернуться назад":
							await ModesHandlers.MainMenuMode(bot, update, clt);
							await ModesHandlers.ChangeModeStatus(update, db, 0);
							break; // <<-- Back to main menu button
						case "Заполнить заново":
							await ModesHandlers.StartUserRegistration(bot, update, clt, db);
							break; // <<-- Field that also start user profile registration
						case "Профиль не заполен полностью":
							await ModesHandlers.TakeData(bot, update, clt, db);
							break; // <<-- Notification that show that profile is not done 
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
