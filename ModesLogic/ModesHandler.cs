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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ModesLogic
{
	public class ModesHandlers
	{
		#region Mainmenu
		public static async Task MainMenuMode(ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken clt)
		{
			long? chatID = TelegramBotUtilities.ReturnChatID(update);
			await TelegramBotUtilities.DisplayMainMenuKeyboard(bot, chatID, "Выберите действие: ", clt);
		}
		#endregion

		#region Profile
		public static async Task ProfileMode(ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken clt)
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

		public static async Task StartUserRegistration(ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken cts, AppDbContext db)
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


		public static async Task TakeData(ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken cts, AppDbContext db)
		{
			var userId = update.Message?.From?.Id ?? update.CallbackQuery?.From?.Id;
			if (userId == null)
				return;

			var regStatus = db.RegistrationStatuses.FirstOrDefault(stat => stat.ProfileId == userId);
			if (regStatus == null)
				return;
			if (regStatus.UserRegStatus == 1)
			{
				await TakeGender(bot, update, cts);
			}
			else if (regStatus.UserRegStatus == 2)
			{
				await TakeGroup(bot, update, cts, db);
			}
			else if (regStatus.UserRegStatus == 3)
			{
				await TakeName(bot, update, cts);
			}
			else if (regStatus.UserRegStatus == 4)
			{
				await TakeLastName(bot, update, cts);
			}
			else if (regStatus.UserRegStatus == 5)
			{
				var keyboard = Keyboards.ReturnFromReg();
				if(update.Message != null)
				{
					var chatid = update.Message.Chat.Id;
					await bot.SendMessage(chatid, )
				}
			}
		}

		private static async Task TakeGender(ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken cts)
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
		private static async Task TakeGroup(ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken cts, AppDbContext db)
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

		private static async Task TakeName(ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken cts)
		{
			long? chatid = TelegramBotUtilities.ReturnChatID(update);
			string? text = "Введите имя:";
			if (chatid != null)
			{
				await bot.SendMessage(
					chatId: chatid,
					text: text,
					cancellationToken: cts
					);
			}
		}
		private static async Task TakeLastName(ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken cts)
		{
			long? chatid = TelegramBotUtilities.ReturnChatID(update);
			string? text = "Введите Фамилию:";
			if (chatid != null)
			{
				await bot.SendMessage(
					chatId: chatid,
					text: text,
					cancellationToken: cts
					);
			}
		}
		public static bool CheckStatus(Telegram.Bot.Types.Update update, AppDbContext db)
		{
			long telegramId = update.Message.From.Id;
			return db.Users.Any(user => user.TelegramID == telegramId);
		}


		#endregion

		#region Answer on profile methods
		public static async Task AnswerOnTakeGender(string data, UserProfile? user, Telegram.Bot.Types.Update update, ITelegramBotClient bot, AppDbContext db, UserRegistrationStatus userregStat)
		{
			user.Gender = data;
			var chatId = update.CallbackQuery.Message.Chat.Id;
			var messageId = update.CallbackQuery.Message.MessageId;
			await bot.DeleteMessage(chatId, messageId);
			userregStat.UserRegStatus = 2;
			await db.SaveChangesAsync();
			await bot.SendMessage(
				chatId,
				$"Ваш пол: {data}",
				replyMarkup: Keyboards.ContinueReg()
				);
		}

		public static async Task AnswerOnTakeGroup(AppDbContext db, string data, Telegram.Bot.Types.Update update, UserProfile user, ITelegramBotClient bot, UserRegistrationStatus userregStat)
		{
			var group = db.Groups.FirstOrDefault(grp => grp.Name == data);
			var messageId = update.CallbackQuery.Message.MessageId;
			var chatId = update.CallbackQuery.Message.Chat.Id;
			if (group != null)
			{
				await bot.DeleteMessage(chatId, messageId);
				user.group = group;
				user.GroupID = group.Id;
				userregStat.UserRegStatus = 3;
				db.SaveChanges();
			}
			await bot.SendMessage(
				chatId,
				$"Вы состоите в группе {data}",
				replyMarkup: Keyboards.ContinueReg()
				);
		}

		public static async Task AnswerOnTakeName(ITelegramBotClient bot, string data, Telegram.Bot.Types.Update update, UserProfile user, AppDbContext db, UserRegistrationStatus userregStat)
		{
			if(data is  string str && str.Length < 150 && data != null)
			{
				user.Name = data;
				db.SaveChanges();
				userregStat.UserRegStatus = 4;
				if (update.Message != null)
				{
					await bot.SendMessage(update.Message.Chat.Id, $"Имя в профиле: {data}", replyMarkup: Keyboards.ContinueReg());
				}
			}
		}

		public static async Task AnswerOnTakeLastName(ITelegramBotClient bot, string data, Telegram.Bot.Types.Update update, UserProfile user, AppDbContext db, UserRegistrationStatus userregStat)
		{
			if (data is string str && str.Length < 150 && data != null)
			{
				user.LastName = data;
				db.SaveChanges();
				userregStat.UserRegStatus = 5;
				if (update.Message != null)
				{
					await bot.SendMessage(update.Message.Chat.Id, $"Фамилия в профиле: {data}", replyMarkup: Keyboards.ContinueReg());
				}
			}
		}
		#endregion
	}
}
