using HelperNamespace;
using HelperNamespce;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace ModesLogic
{
	public class ApplicationsHandler
	{
		public static async Task WarningUser(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			long userId = update.Message.From.Id;

			await bot.SendMessage(userId, TelegramBotUtilities.StudentsWarning(), replyMarkup: Keyboards.ConfirmButton());
		}


		#region Data takers
		public static async Task TakeApplication(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			long userId = update.Message.From.Id;

			if (!await db.RegistrationStatuses.AnyAsync(reg => reg.TelegramId == userId))
			{
				await db.RegistrationStatuses.AddAsync(new UserRegistrationService { TelegramId = userId, AppStatus = 0 });
				await db.SaveChangesAsync();
			}

			var regStat = await db.RegistrationStatuses.FirstOrDefaultAsync(reg => reg.TelegramId == userId);
			if (regStat == null)
				return;

			switch (regStat.AppStatus)
			{
				case 0:
					await TakeMaleLyceum(bot, update, db);
					return;
				case 1:
					await TakeMaleFullName(bot, update, db);
					return;
				case 2:
					await TakeMalePhoneNumber(bot, update, db);
					return;
				case 3:
					return;
				case 4:
					return;
			}
		}

		public static async Task TakeMaleLyceum(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null) 
				return;

			long userId = update.Message.From.Id;

			await bot.SendMessage(userId, 
				"Выберите лицей парня:", 
				replyMarkup: Keyboards.ChooseLyceum()
				);
		}
		public static async Task TakeMaleFullName(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			long userId = update.Message.From.Id;

			await bot.SendMessage(userId, TelegramBotUtilities.ReturnMDataText(), replyMarkup: Keyboards.ContinueApplicationSend(), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
		}
		public static async Task TakeMalePhoneNumber(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			long userId = update.Message.From.Id;

			await bot.SendMessage(
				userId, 
				TelegramBotUtilities.ReturnPhoneNumberText(), 
				replyMarkup: Keyboards.ContinueOrReturnButton(),
				parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
				);
		}
		public static async Task TakePurpose(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null) 
				return;

			long userId = update.Message.From.Id;

			await bot.SendMessage(userId,
				"Почему вы хотите пойти на зимний бал?",
				replyMarkup: Keyboards.ContinueOrReturnButton()
				);
		}
		
			public static async Task TakeFemaleFullName(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			long userId = update.Message.From.Id;

			await db.SaveChangesAsync();

			await bot.SendMessage(userId, TelegramBotUtilities.ReturnMDataText(), replyMarkup: Keyboards.ContinueApplicationSend(), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
		}
		

		#endregion

		#region Answer on update
		public static async Task AnswerOnLyceumTake(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db, UserRegistrationService userReg)
		{
			if (update?.Message?.From == null) 
				return;

			long userId = update.Message.From.Id;
			string? data = update.Message.Text;
			if (data == null)
				return;

			if (!await db.Applications.AnyAsync(reg => reg.TelegramID == update.Message.From.Id))
			{
				await db.Applications.AddAsync(new Application { TelegramID = userId });
				await db.SaveChangesAsync();
			}

			var application = await db.Applications.FirstOrDefaultAsync(reg => reg.TelegramID == userId);
			if (application == null)
				return;

			userReg.AppStatus = 1;
			application.MaleLyceumName = data;
			await db.SaveChangesAsync();
			await TakeApplication(bot, update, db);
		}
		public static async Task AnswerOnMFullName(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db, UserRegistrationService userReg)
		{
			if (update?.Message?.From == null)
				return;

			long userId = update.Message.From.Id;
			string? data = update.Message.Text;
			if (data == null)
				return;

			var application = await db.Applications.FirstOrDefaultAsync(app => app.TelegramID == userId);
			if (application == null)
				return;

			userReg.AppStatus = 2;
			application.MaleFullName = data;
			await db.SaveChangesAsync();

			await bot.SendMessage(
				userId, 
				TelegramBotUtilities.ReturnFullNameConfirmationText(data), 
				replyMarkup: Keyboards.ContinueOrReturnButton(),
				parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
		}
		public static async Task AnswerOnMPhoneNubmer(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db, UserRegistrationService userReg)
		{
			if (update?.Message?.From == null)
				return;

			long userId = update.Message.From.Id;
			string? data = update.Message.Text;
			if(data == null) 
				return;

			var application = await db.Applications.FirstOrDefaultAsync(app => app.TelegramID == userId);
			if (application == null) 
				return;

			userReg.AppStatus = 3;
			application.MaleTelegramUserAndPhoneNumber = $"user: {update.Message.From.Username} pn: {data}";
			await db.SaveChangesAsync();

			await bot.SendMessage(
				userId, 
				TelegramBotUtilities.ReturnPhoneNumberAndUsernameConfirmText(data), 
				replyMarkup: Keyboards.ContinueOrReturnButton(),
				parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
				);
		}
		public static async Task AnswerOnMPurpose(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db, UserRegistrationService userReg)
		{
			if (update?.Message?.From == null) 
				return;

			long userId = update.Message.From.Id;
			string? data = update.Message.Text;
			if (data == null)
				return;

			var application = await db.Applications.FirstOrDefaultAsync(app => app.TelegramID == userId);
			if(application == null) 
				return;

			application.MalePurpose = data;
			userReg.AppStatus = 4;

			await db.SaveChangesAsync();

			await bot.SendMessage(
				userId, 
				
				);
		}
		#endregion

		public static async Task Return(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if(update?.Message?.From == null) 
				return;

			var userId = update.Message.From.Id;

			var userReg = await db.RegistrationStatuses.FirstOrDefaultAsync(reg => reg.TelegramId == userId);
			if(userReg == null) 
				return;

			if(userReg.AppStatus > 0)
			{
				userReg.AppStatus -= 1;
				await db.SaveChangesAsync();
			}

			await ApplicationsHandler.TakeApplication(bot, update, db);
		}
	}
}
