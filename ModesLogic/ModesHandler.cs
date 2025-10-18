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
			var username = TelegramBotUtilities.ReturnUsername(update);
			Message message = await TelegramBotUtilities.DisplayMainMenuKeyboard(bot, chatID, "Выберите действие: ", clt);
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
				Message message = await bot.SendMessage(
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

		public async Task StartUserRegistration(ITelegramBotClient bot, Update update, CancellationToken cts, UserProfile user, AppDbContext db)
		{
			long? chatID = TelegramBotUtilities.ReturnChatID(update);
			var keyboard = Keyboards.StartRegistrationKeyboard();
			string TextToSend = TelegramBotUtilities.StartRegirstrationText();
			if (TextToSend != null && chatID != null)
			{
				var message = await bot.SendMessage(
					chatId: chatID,
					text: TextToSend,
					replyMarkup: keyboard,
					cancellationToken: cts
					);
			}
			if(update.Message != null && update.Message.Text != null)
			{
				if(update.Message.Text == TextToSend)
				{
					await TakeData(bot, update, cts, db);
				}
			}
		}


		private async Task TakeData(ITelegramBotClient bot, Update update, CancellationToken cts, AppDbContext db)
		{
			UserProfile user = new UserProfile();
			await TakeGender(bot, update, cts, user);
			await TakeGroup(bot, update, cts, user, db);

			db.Users.Add(user);
			db.SaveChanges();
		}


		private async static Task TakeGender(ITelegramBotClient bot, Update update, CancellationToken cts, UserProfile user)
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
			var callbackData = update.CallbackQuery;
			string? data = callbackData?.Data;

			if (callbackData != null && data != null)
			{
				if(data == "Male")
				{
					user.Gender = "Male";
				}
				else if(data == "Female")
				{
					user.Gender = "Female";
				}
			}
		}
		private async static Task TakeGroup(ITelegramBotClient bot, Update update, CancellationToken cts, UserProfile user, AppDbContext db)
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

			var callbackData = update.CallbackQuery;
			string? data = callbackData?.Data;
			var findgroup = db.Groups.ToList().FirstOrDefault(grp => grp.Name == data);
			if(findgroup != null) 
			{
				user.group = findgroup;
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
