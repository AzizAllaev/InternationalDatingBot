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
using MainMenu;
using Profile;


namespace HandleUpdate
{
	public static class HandleUpdatesMethods
	{
		public static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken clt)
		{
			if (update.Message == null)
				return;
			string text = HelperNamespace.HelperMethods.ReturnNewMessage(update);
			switch (text)
			{
				case "/start":
					await MainMenuMethods.MainMenuMode(bot, update, clt);
					break;
				case "Профиль👤":
					await ProfileModeMethods.ProfileMode(bot, update, clt);
					break;
				case "Выбор кандидата🪩":
					break;
				case "Убарть себя из списка📌":
					break;
				case "Вернуться назад":
					await MainMenuMethods.MainMenuMode(bot, update, clt);
					break;
			}
		}

	}
}
