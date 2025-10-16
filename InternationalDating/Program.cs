using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot;
using Handlers;
using Models;

namespace InternationalDating
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var bot = new TelegramBotClient("7709122756:AAHs1xvGGrShZ3U0MrJTKMwVsWGjJCfgdls");
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
