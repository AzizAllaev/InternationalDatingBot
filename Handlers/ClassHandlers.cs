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



namespace Handlers
{
	public class ClassHandlers
	{
		public static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken clt)
		{
			if (update.Message == null)
				return;
			string? text = TelegramBotUtilities.ReturnNewMessage(update);
			if (text != null)
			{
				switch (text)
				{
					case "/start":
						await ModesHandlers.MainMenuMode(bot, update, clt);
						break;
					case "Профиль👤":
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
		public static async Task HandleError(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken token)
		{
			Console.WriteLine("Ошибка кода");
			throw new Exception("Фатальная ошибка, бот временно прекращает работу");
		}

	}
}
