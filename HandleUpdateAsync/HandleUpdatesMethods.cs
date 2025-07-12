using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;
using ModesLogic;


namespace HandleUpdate
{
	public static class HandleUpdatesMethods
	{
		public static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken clt)
		{
			if (update.Message == null)
				return;
			string text = HelperNamespace.TelegramBotUtilities.ReturnNewMessage(update);
			switch (text)
			{
				case "/start":
					await ModesHandler.MainMenuMode(bot, update, clt);
					break;
				case "Профиль👤":
					await ModesHandler.ProfileMode(bot, update, clt);
					break;
				case "Выбор кандидата🪩":
					break;
				case "Убарть себя из списка📌":
					break;
				case "Вернуться назад":
					await ModesHandler.MainMenuMode(bot, update, clt);
					break;
			}
		}

	}
}
