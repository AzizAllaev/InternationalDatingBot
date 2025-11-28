using HelperNamespace;
using HelperNamespce;
using Microsoft.EntityFrameworkCore;
using Models;
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



namespace Handlers
{
	public class ClassHandlers
	{
		public static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken clt)
		{
			try
			{
				using AppDbContext db = new AppDbContext();

				#region Profile registration field
				if (update.CallbackQuery != null && update.CallbackQuery.Data != null)
				{
					await ModesLogic.RespondHandlers.WhenCallBackquery(bot, update);
				}
				if (update.Message != null && update.Message.From != null)
				{
					await ModesLogic.RespondHandlers.WhenMessageForProfile(bot, update);
				}
				if (update.Message != null && update.Message.Photo != null)
				{
					await ModesLogic.RespondHandlers.WhenPhotoForProfile(bot, update);
				}
				#endregion

				if (update?.Message?.From != null)
				{
					string? text = TelegramBotUtilities.ReturnNewMessage(update);
					if (text != null)
					{
						switch (text)
						{
							case "/start":
								await bot.SendMessage(update.Message.From.Id, "Выберите действие: ", replyMarkup: Keyboards.MainOptions());
								break;

							// Respond on main buttons of modes
							case "Оставить заявку🪧":
								//await bot.SendMessage(update.Message.From.Id, "Регистрация пока ещё не открылась");
								await bot.SendMessage(update.Message.From.Id, TelegramBotUtilities.StudentsWarning(), replyMarkup: Keyboards.ConfirmButton());
								break;
							case "Я прочитал":
								await ApplicationsHandler.TakeApplication(bot, update, db);
								break;
							case "Подтверждаю☑️":
								await ApplicationsHandler.TakeApplication(bot, update, db);
								break;
							case "Дополнительные функции":
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
								await ModesHandlers.ChangeModeStatus(update, db, 1);
								await ModesHandlers.ProfileMode(bot, update, clt, db);
								break; // !!! Field that send to user UserProfile !!!
							case "Убарть себя из списка📌":
								await ModesHandlers.DeleteUser(bot, update, db);
								break; // !!! Field that delete all data about user from DB !!!
							case "Выбор кандидата🪩":
								await ModesHandlers.ChangeModeStatus(update, db, 2);
								await ModesHandlers.PartnerShowcaseMenu(bot, update, db);
								break; // !!! Field that start partner showcase !!!
							case "Назад🔙":
								await bot.SendMessage(update.Message.From.Id, "Выберите действие: ", replyMarkup: Keyboards.MainOptions());
								break;

							// Respond on service buttons
							case "Данные анкеты👁️":
								await ModesHandlers.ChangeModeStatus(update, db, 1);
								await ModesHandlers.TakeData(bot, update, clt, db);// <<--- This methods start registration
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

							// Respond on partner showcase buttons
							case "Поиск пары🎆":
								await ModesHandlers.ChangeModeStatus(update, db, 4);
								await ModesHandlers.FindPair(bot, update, db);
								break;
							case "Кто меня лайкнул⁉️":
								await ModesHandlers.ChangeModeStatus(update, db, 3);
								await ModesHandlers.ViewLikes(bot, update, db);
								break;
							case "👍":
								await ModesHandlers.HandleLike(bot, update, db);
								break;
							case "👎":
								await ModesHandlers.HandleDislike(bot, update, db);
								break;
						}

						if(await ModesHandlers.ReturnModeStatus(update, db) == 3 && text != "Кто меня лайкнул⁉️")
						{
							await ModesHandlers.MatchUser(bot, update, db);
						}

						#region Application methods
						var regStat = await db.RegistrationStatuses.FirstOrDefaultAsync(reg => reg.TelegramId == update.Message.From.Id);
						if (regStat != null)
						{
							if(regStat.AppStatus < 3)
							{
								await RespondHandlers.WhenDataOfMale(bot, update, db);
							}
						}
						#endregion
					}
				}
			}
			catch(Exception ex) 
			{
				Console.WriteLine($"Ошибка в HandleUpdateAsync: {ex.Message}");
				Console.WriteLine(ex.StackTrace);
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
