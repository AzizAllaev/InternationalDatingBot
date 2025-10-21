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

namespace ModesLogic
{
	public class ModesHandlers
	{
		#region Mainmenu
		public static async Task MainMenuMode(ITelegramBotClient bot, Update update, CancellationToken clt)
		{
			long? chatID = TelegramBotUtilities.ReturnChatID(update);
			await TelegramBotUtilities.DisplayMainMenuKeyboard(bot, chatID, "Выберите действие: ", clt);
		}
		#endregion

		#region Profile
		public static async Task ProfileMode(ITelegramBotClient bot, Update update, CancellationToken clt)
		{
			long? chatID = TelegramBotUtilities.ReturnChatID(update);
			var keyboard = Keyboards.MakeReturnKeyboard();
			string? username= TelegramBotUtilities.ReturnUsername(update);
			string? messageForButton = TelegramBotUtilities.ReturnProfileText(username);
			if (messageForButton != null && chatID != null)
			{
				await bot.SendMessage(
					chatId: chatID.Value,
					text: messageForButton,
					replyMarkup: keyboard,
					cancellationToken: clt
					);
			}
		}
		#endregion 

		#region SelectMenu
		#endregion

		#region Make proifle

		public static async Task StartUserRegistration(ITelegramBotClient bot, Update update, CancellationToken cts, AppDbContext db)
		{
			if (update.Message != null && update.Message.From != null)
			{
				var RegistrationStat = new UserRegistrationStatus();
				RegistrationStat.ProfileId = update.Message.From.Id;
				RegistrationStat.UserRegStatus = 1;
				db.RegistrationStatuses.Add(RegistrationStat);
				db.SaveChanges();
			}
			long? chatID = TelegramBotUtilities.ReturnChatID(update);
			var keyboard = Keyboards.StartRegistrationKeyboard();
			string TextToSend = TelegramBotUtilities.StartRegirstrationText();
			if (TextToSend != null && chatID != null)
			{
				await bot.SendMessage(
					chatId: chatID,
					text: TextToSend,
					replyMarkup: keyboard,
					cancellationToken: cts
					);
			}
		}

		public static async Task TakeData(ITelegramBotClient bot, Update update, CancellationToken cts, AppDbContext db)
		{
			if (update.CallbackQuery != null)
			{
				var a = db.RegistrationStatuses.FirstOrDefault(stat => stat.ProfileId == update.CallbackQuery.From.Id);
				if(a.UserRegStatus == 1)
				{
					a.UserRegStatus = 2;
					//await TakeGender(bot, update, cts, );
				}
				else if(a.UserRegStatis == 2)
                {
					//await TakeGroup(bot, update, cts, )
                }
			}
		}

		public static async Task TakeGender(ITelegramBotClient bot, Update update, CancellationToken cts)
		{
			var keyboard = Keyboards.TakeGenderKeyboard();
			long? chatid = TelegramBotUtilities.ReturnChatID(update);
			if (chatid != null)
			{
				await bot.SendMessage(
					chatId: chatid,
					text: "Ваш пол: ",
					replyMarkup: keyboard,
					cancellationToken: cts
					);
			}
		}
		public static async Task TakeGroup(ITelegramBotClient bot, Update update, CancellationToken cts)
		{
			long? chatid = TelegramBotUtilities.ReturnChatID(update);
			var keyboard = Keyboards.TakeGroupKeyboard(db.Groups.ToList());
			string? text = TelegramBotUtilities.AskGroupText();
			if (chatid != null && keyboard != null)
			{
				await bot.SendMessage(
					chatId: chatid,
					text: text,
					replyMarkup: keyboard,
					cancellationToken: cts
					);
			}
		}

		public static bool CheckStatus(Update update, AppDbContext db)
		{
			if (update.Message?.From == null)
				return false; 
			long telegramId = update.Message.From.Id;
			return db.Users.Any(user => user.TelegramID == telegramId);
		}

		#endregion
	}
}
