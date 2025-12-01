using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;
using HelperNamespace;
using HelperNamespce;
using Models;
using Microsoft.Extensions.Logging.Abstractions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;

namespace ModesLogic
{
	public class RespondHandlers
	{
		#region Profile respond methods
		public static async Task WhenCallBackquery(ITelegramBotClient bot, Telegram.Bot.Types.Update update)
		{
			if (update?.CallbackQuery?.Data == null)
				return;

			await bot.AnswerCallbackQuery(update.CallbackQuery.Id);
			string CallBackqueryData = update.CallbackQuery.Data;
			using var db = new AppDbContext();
			var user = await db.Users.FirstOrDefaultAsync(u => u.TelegramID == update.CallbackQuery.From.Id);
			var userregStat = await db.RegistrationStatuses.FirstOrDefaultAsync(ureg => ureg.TelegramId == update.CallbackQuery.From.Id);
			if (CallBackqueryData != null && user != null && userregStat != null && update.CallbackQuery.Message != null)
			{
				if (CallBackqueryData == "Male" || CallBackqueryData == "Female" && userregStat.UserRegStatus == 1)
				{
					await ModesHandlers.AnswerOnTakeGender(CallBackqueryData, user, update, bot, db, userregStat);
				}
				else if (db.Groups.Any(grp => grp.Name == CallBackqueryData && userregStat.UserRegStatus == 2))
				{
					await ModesHandlers.AnswerOnTakeGroup(bot, CallBackqueryData, update, db, userregStat, user);
				}
			}
		}

		public static async Task WhenMessageForProfile(ITelegramBotClient bot, Telegram.Bot.Types.Update update)
		{

			if (update?.Message?.From == null)
				return;

			using var db = new AppDbContext();
			var userregStat = await db.RegistrationStatuses.FirstOrDefaultAsync(ureg => ureg.TelegramId == update.Message.From.Id);
			if (userregStat != null && update?.Message?.From != null)
			{
				var user = await db.Users.FirstOrDefaultAsync(u => u.TelegramID == update.Message.From.Id);
				if (update.Message.Text != null && update.Message.Text != "Подтверждаю✅")
				{
					if (userregStat.UserRegStatus == 3)
					{
						if (user != null)
							await ModesHandlers.AnswerOnTakeName(bot, update.Message.Text, update, user, db, userregStat);
					}
					else if (userregStat.UserRegStatus == 4)
					{
						if (user != null)
							await ModesHandlers.AnswerOnTakeLastName(bot, update.Message.Text, update, user, db, userregStat);
					}
				}
				await db.SaveChangesAsync();
			}
		}

		public static async Task WhenPhotoForProfile(ITelegramBotClient bot, Telegram.Bot.Types.Update update)
		{
			using var db = new AppDbContext();
			if (update.Message != null && update.Message.From != null)
			{
				var regStat = await db.RegistrationStatuses.FirstOrDefaultAsync(reg => reg.TelegramId == update.Message.From.Id);
				if (regStat != null && regStat.UserRegStatus == 5)
				{
					await ModesHandlers.AnswerOnTakePhoto(bot, update, db, regStat);
				}
			} 
		}
		#endregion

		#region Application respond methods
		public static async Task WhenDataOfMale(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db, UserRegistrationService userRegStat)
		{
			if (update?.Message?.From == null)
				return;
			long userId = update.Message.From.Id;
			if (userRegStat == null)
				return;
			
			switch (userRegStat.AppStatus)
			{
				case 0:
					await ApplicationsHandler.AnswerOnMLyceumTake(bot, update, db, userRegStat);
					return;
				case 1:
					await ApplicationsHandler.AnswerOnMFullName(bot, update, db, userRegStat);
					return;
				case 2:
					await ApplicationsHandler.AnswerOnMPhoneNubmer(bot, update, db, userRegStat);
					return;
				case 3:
					await ApplicationsHandler.AnswerOnMPurpose(bot, update, db, userRegStat);
					return;
				case 4:
					await ApplicationsHandler.AnswerOnFLyceumTake(bot, update, db, userRegStat);
					return;
				case 5:
					await ApplicationsHandler.AnswerOnFFullName(bot, update, db, userRegStat);
					break;
				case 6: 
					await ApplicationsHandler.AnswerOnFPhoneNumber(bot, update, db, userRegStat);
					return;
				case 7:
					return;
				case 8:
					return;
			}
		}
		#endregion
	}
}
