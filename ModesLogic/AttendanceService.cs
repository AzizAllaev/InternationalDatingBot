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
	public class AttendanceService
	{
		public static async Task SetAttendance(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			long userId = update.Message.From.Id;

			await bot.SendMessage(userId, TelegramBotUtilities.StudentsWarningatAttendance(), replyMarkup: Keyboards.StartAttendance());
		}

		#region Takers
		public static async Task AttendanceTaker(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;
			
			long userId = update.Message.From.Id;

			if (!await db.RegistrationStatuses.AnyAsync(reg => reg.TelegramId == userId))
			{

				await db.RegistrationStatuses.AddAsync(new UserRegistrationService { TelegramId = userId, AttendanceStatus = 0 });
				await db.SaveChangesAsync();
			}
			var regStat = await db.RegistrationStatuses.FirstOrDefaultAsync(reg => reg.TelegramId == userId);
			if (regStat == null)
				return;

			switch (regStat.AttendanceStatus)
			{
				case 0:
					await TakeLyceum(bot, update);
					break;
				case 1:
					await TakeFullName(bot, update);
					return;
				case 2:
					await ConfirmAttendance(bot, update, db);
					return;
				case 3:
					await bot.SendMessage(userId, "<b><i>Ваши данные отправлены</i></b>✅", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: Keyboards.MainOptions());
					return;
			}
		}

		public static async Task TakeLyceum(ITelegramBotClient bot, Telegram.Bot.Types.Update update)
		{
			if (update?.Message?.From == null)
				return;

			long userId = update.Message.From.Id;

			await bot.SendMessage(userId,
				"Выберите ваш лицей:",
				replyMarkup: Keyboards.ChooseLyceum()
				);
		}
		public static async Task TakeFullName(ITelegramBotClient bot, Telegram.Bot.Types.Update update)
		{
			if (update?.Message?.From == null)
				return;

			long userId = update.Message.From.Id;

			await bot.SendMessage(userId, TelegramBotUtilities.ReturnAttendanceText(), Telegram.Bot.Types.Enums.ParseMode.Html);
		}
		#endregion

		#region Respond methods
		public static async Task AnswerOnLyceum(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db, UserRegistrationService userReg)
		{
			if (update?.Message?.From == null)
				return;

			long userId = update.Message.From.Id;
			string? data = update.Message.Text;

			if (data == null)
				return;

			if (!await db.Attendances.AnyAsync(att => att.TelegramID == userId))
			{
				await db.Attendances.AddAsync(new Attendance { TelegramID = userId });
				await db.SaveChangesAsync();
			}

			var attendance = await db.Attendances.FirstOrDefaultAsync(reg => reg.TelegramID == userId);
			if (attendance == null)
				return;

			attendance.LyceumName = data;
			userReg.AttendanceStatus = 1;
			await db.SaveChangesAsync();
			await AttendanceTaker(bot, update, db);
		}
		public static async Task AnswerOnAttendance(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db, UserRegistrationService userReg)
		{
			if (update?.Message?.From == null)
				return;

			long userId = update.Message.From.Id;
			string? data = update.Message.Text;

			if (data == null)
				return;

			var attendance = await db.Attendances.FirstOrDefaultAsync(reg => reg.TelegramID == userId);
			if (attendance == null)
				return;

			attendance.FullNameAndGroup = data;
			userReg.AttendanceStatus = 2;
			await db.SaveChangesAsync();
			await AttendanceTaker(bot, update, db);
		}
		public static async Task SendAttendance(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			long userId = update.Message.From.Id;
			var attendance = await db.Attendances.FirstOrDefaultAsync(att => att.TelegramID == userId);
			var userreg = await db.RegistrationStatuses.FirstOrDefaultAsync(reg => reg.TelegramId == userId);
			if (attendance == null || userreg == null)
				return;

			var service = GoogleApiHandler.ConnectToSheets(@"C:\Enviromentals\plucky-sector-449218-h4-c705fa2c3892.json");
			if (attendance.Status != "Sended")
			{
				await GoogleApiHandler.AddAttendanceRow(service, attendance, "1iH-mAFuS0jKeMLxfc0lO3Lk-zLo8K7czOjIhM2_zbA8", "Лист2!A1");
				attendance.Status = "Sended";
				userreg.AttendanceStatus = 3;
				await db.SaveChangesAsync();
				await bot.SendMessage(userId, "<b><i>Ваши данные отправлены</i></b>✅", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: Keyboards.MainOptions());
				return;

			}
			else
			{
				await bot.SendMessage(userId, "<b><i>Ваши данные отправлены</i></b>✅", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: Keyboards.MainOptions());
				return;
			}
		}
		#endregion

		#region Confirm attendance
		public static async Task ConfirmAttendance(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if(update?.Message?.From == null)
				return;

			long userId = update.Message.From.Id;

			var attendance = await db.Attendances.FirstOrDefaultAsync(att => att.TelegramID == userId); 
			if (attendance == null) 
				return;

			await bot.SendMessage(userId, $"Подтвердите данные: \n <b>{attendance.FullNameAndGroup}</b>", replyMarkup: Keyboards.ConfirmAttendance(), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
		}
		public static async Task MakeAttendanceAgain(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			long userId = update.Message.From.Id;

			var userReg = await db.RegistrationStatuses.FirstOrDefaultAsync(reg => reg.TelegramId == userId);
			if (userReg == null) 
				return;

			userReg.AttendanceStatus = 0;
			await db.SaveChangesAsync();

			await AttendanceTaker(bot, update, db);
		}
		#endregion
	}
}
