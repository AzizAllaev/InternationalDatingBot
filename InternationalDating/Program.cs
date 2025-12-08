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
			AppDbContext.EnsureDatabaseCreated();

			string token = File.ReadAllText(@"C:\bot\token.txt");
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
