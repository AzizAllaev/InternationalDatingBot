using Telegram.Bot;
using HandleUpdate;
using HandleError;

namespace InternationalDating
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var bot = new TelegramBotClient("7709122756:AAEYC7knJSLerAhf-QhiNg2rnrHFfBSCSeg");
			using var cts = new CancellationTokenSource();

			bot.StartReceiving(
				HandleUpdatesMethods.HandleUpdateAsync,
				HandleErrorMethods.HandleErrorAsync,
				cancellationToken: cts.Token
				);

			Console.ReadLine();
		}
	}
}
