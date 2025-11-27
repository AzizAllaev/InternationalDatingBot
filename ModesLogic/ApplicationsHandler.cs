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
			
			await bot.SendMessage(userId, "⚠️Убедительная просьба, старайтесь вводить данные правильно. Перед отправкой проверьте и перечитайте заявку и после этого отправляйте её.⚠️");


		}

		public static async Task TakeApplication(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			long userId = update.Message.From.Id;

			if(!await db.RegistrationStatuses.AnyAsync(reg => reg.TelegramId == userId))
			{
				await db.RegistrationStatuses.AddAsync(new UserRegistrationService { TelegramId = userId });
				await db.SaveChangesAsync();
			}

			var regStat = await db.RegistrationStatuses.FirstOrDefaultAsync(reg => reg.TelegramId == userId);
			if (regStat == null)
				return;

			switch(regStat.AppStatus)
			{
				case 0:

					break;
				case 1:
					break;
				case 2:
					break;
				case 3:
					break;
			}
		}
	}
}
