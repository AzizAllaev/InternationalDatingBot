using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot;
using Handlers;
using Models;

namespace InternationalDating
{
    internal class Program
    {
        static void Main(string[] args)
        {
			var token = Environment.GetEnvironmentVariable("BOT_TOKEN");
			if (token == null)
				return;
			var bot = new TelegramBotClient(token);
			using var cts = new CancellationTokenSource();

			bot.StartReceiving(
				ClassHandlers.HandleUpdateAsync,
				ClassHandlers.HandleError, 
				new Telegram.Bot.Polling.ReceiverOptions(), 
				cts.Token);

			Console.ReadLine();
		}

	}
}
