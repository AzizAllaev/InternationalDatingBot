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
			var keyboard = Keyboards.ReturnFromProfile();
			string? username = TelegramBotUtilities.ReturnUsername(update);
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
				if (!db.Users.Any(reg => reg.TelegramID == update.Message.From.Id))
				{
					UserProfile user = new UserProfile();
					user.Username = update.Message.From.Username;
					user.TelegramID = update.Message.From.Id;
					db.Users.Add(user);
					db.SaveChanges();
				}
				if (!db.RegistrationStatuses.Any(reg => reg.ProfileId == update.Message.From.Id))
				{
					var RegistrationStat = new UserRegistrationStatus();
					RegistrationStat.ProfileId = update.Message.From.Id;
					RegistrationStat.UserRegStatus = 1;
					db.RegistrationStatuses.Add(RegistrationStat);
					db.SaveChanges();
				}
			}
			if (update.Message != null && update.Message.From != null)
			{
				if (db.RegistrationStatuses.Any(stat => stat.ProfileId == update.Message.From.Id))
				{
					var regstat = db.RegistrationStatuses.FirstOrDefault(stat => stat.ProfileId == update.Message.From.Id);
					if (regstat != null)
					{
						await bot.SendMessage(update.Message.Chat.Id, "тест");
						regstat.UserRegStatus = 1;
						db.SaveChanges();
					}
				}
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
			await bot.SendMessage(update.Message.Chat.Id, "Метод получения группы запустился");
			if (update.Message != null && update.Message.From != null)
			{
				var a = db.RegistrationStatuses.FirstOrDefault(stat => stat.ProfileId == update.Message.From.Id);
				if (a != null)
				{

					if (a.UserRegStatus == 1)
					{
						await TakeGender(bot, update, cts, db);
					}
					else if (a.UserRegStatus == 2)
					{
						await bot.SendMessage(update.Message.Chat.Id, "Метод получения группы запустился");
						await TakeGroup(bot, update, cts, db);
					}
					else if(a.UserRegStatus == 3)
					{
						await bot.SendMessage(update.Message.Chat.Id, "пошел нахуй");
					}
				}
			}
		}

		private static async Task TakeGender(ITelegramBotClient bot, Update update, CancellationToken cts, AppDbContext db)
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
				var stat = db.RegistrationStatuses.FirstOrDefault(st => st.ProfileId == update.Id);
				if (stat != null)
				{
					stat.UserRegStatus = 2;
				}
			}
		}
		private static async Task TakeGroup(ITelegramBotClient bot, Update update, CancellationToken cts, AppDbContext db)
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
			long telegramId = update.Message.From.Id;
			return db.Users.Any(user => user.TelegramID == telegramId);
		}

		#endregion
	}
}
