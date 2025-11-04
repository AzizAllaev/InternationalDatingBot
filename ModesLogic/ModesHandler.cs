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
using Microsoft.EntityFrameworkCore;

namespace ModesLogic
{
	public class ModesHandlers
	{
		#region Mainmenu
		public static async Task MainMenuMode(ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken cts)
		{
			if (update.Message?.From == null)
				return;

			var keyboard = Keyboards.MakeMainMenuKeyboard();
			await bot.SendMessage(
				chatId: update.Message.Chat.Id,
				text: "Выберите действие:",
				replyMarkup: keyboard,
				cancellationToken: cts
				);
		}
		#endregion

		#region Profile
		public static async Task ProfileMode(ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken clt, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			long? chatID = TelegramBotUtilities.ReturnChatID(update);
			var keyboard = Keyboards.ReturnFromProfile();
			string? username = TelegramBotUtilities.ReturnUsername(update);
			string? messageForButton = await TelegramBotUtilities.ReturnBaseProfileText(db, update);
			var user = await db.Users.FirstOrDefaultAsync(x => x.TelegramID == update.Message.From.Id);

			if (messageForButton != null && chatID != null && user != null && user.PhotoId != null && user.Name != null)
			{
				await bot.SendPhoto(
					chatId: chatID,
					caption: messageForButton,
					replyMarkup: keyboard,
					cancellationToken: clt,
					photo: user.PhotoId
					);
			}
			else
			{
				if (chatID == null)
					return;
				await bot.SendMessage(chatID, "Профиль не заполнен полностью", replyMarkup: Keyboards.ReturnFromProfile());
			}
		}
		#endregion 

		#region Select partner menu
		public static async Task PartnerShowcaseMenu(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			if (!await TelegramBotUtilities.CheckProfileFill(update, db))
			{
				await bot.SendMessage(update.Message.Chat.Id, "Пожалуйста заполните профиль полностью");
				return;
			}
			await bot.SendMessage(update.Message.Chat.Id,
				"Выберите действие: ",
				replyMarkup: Keyboards.ChooseModeInPS()
				);
		}

		public static async Task FindPair(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			if (!await db.TargetPartnerServices.AnyAsync(t => t.TelegramID == update.Message.From.Id))
			{
				await db.TargetPartnerServices.AddAsync(new TargetPartnerService { TelegramID = update.Message.From.Id, LastUserId = 0 });
				await db.SaveChangesAsync();
			}

			var target = await db.TargetPartnerServices.FirstOrDefaultAsync(t => t.TelegramID == update.Message.From.Id);
			var baseuser = await db.Users.FirstOrDefaultAsync(u => u.TelegramID == update.Message.From.Id);
			if (target == null || baseuser == null)
				return;

			var targetuser = await db.Users.FirstOrDefaultAsync(tu => tu.Id > target.LastUserId && tu.Gender != baseuser.Gender);
			if (targetuser == null)
			{
				await bot.SendMessage(update.Message.Chat.Id, "К сожелению я никого не нашел тебе😔");
				return;
			}
			if (!TelegramBotUtilities.CheckProfileFillByUser(update, targetuser))
				return;

			await bot.SendPhoto(
				chatId: update.Message.Chat.Id,
				caption: await TelegramBotUtilities.ReturnTargetProfileText(targetuser, db),
				replyMarkup: Keyboards.LikeDislikeButtons(),
				photo: targetuser.PhotoId
				);

			target.LastUserId = targetuser.Id;
		}

		public static async Task HandleLike(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			var target = await db.TargetPartnerServices.FirstOrDefaultAsync(t => t.TelegramID == update.Message.From.Id);

			if(target == null) 
				return;
			
			var baseuser = await db.Users.FirstOrDefaultAsync(u => u.TelegramID == update.Message.From.Id);
			if (baseuser == null) 
				return;

			var targetuser = await db.Users.FirstOrDefaultAsync(u => u.Id == target.LastUserId || u.Gender != baseuser.Gender);
			if (targetuser == null || target.TelegramID == 0)
				return;

			Console.WriteLine($"Лайкнули: {targetuser.Name} || {targetuser.Gender} || {targetuser.TelegramID}");
			await bot.SendMessage(targetuser.TelegramID, "Тебе поставили лайк, проверь приглашения🗣️");


			if (baseuser.Gender == "Female")
			{
				if (await db.Likes.AnyAsync(l => l.FemaleId == baseuser.Id))
				{
					return;
				}
			}
			else if(baseuser.Gender == "Male")
			{
				if (await db.Likes.AnyAsync(l => l.MaleId == baseuser.Id))
				{
					return;
				}
			}
			
			Like like = new Like();
			if (targetuser.Gender == "Female")
			{
				like.FemaleId = targetuser.Id;
				like.MaleId = baseuser.Id;
			}
			else if(targetuser.Gender == "Male")
			{
				like.FemaleId = baseuser.Id;
				like.MaleId = targetuser.Id;
			}
			await db.Likes.AddAsync(like);
			await db.SaveChangesAsync();
		}
		#endregion

		#region Make proifle

		public static async Task StartUserRegistration(ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken cts, AppDbContext db)
		{
			if (update.Message != null && update.Message.From != null)
			{
				if (!await db.Users.AnyAsync(reg => reg.TelegramID == update.Message.From.Id))
				{
					UserProfile user = new UserProfile();
					user.Username = update.Message.From.Username;
					user.TelegramID = update.Message.From.Id;
					await db.Users.AddAsync(user);
					await db.SaveChangesAsync();
				}
				if (!db.RegistrationStatuses.Any(reg => reg.TelegramId == update.Message.From.Id))
				{
					var RegistrationStat = new UserRegistrationService();
					RegistrationStat.TelegramId = update.Message.From.Id;
					RegistrationStat.UserRegStatus = 1;
					await db.RegistrationStatuses.AddAsync(RegistrationStat);
					await db.SaveChangesAsync();
				}
			}
			if (update.Message != null && update.Message.From != null)
			{
				if (db.RegistrationStatuses.Any(stat => stat.TelegramId == update.Message.From.Id))
				{
					var regstat = await db.RegistrationStatuses.FirstOrDefaultAsync(stat => stat.TelegramId == update.Message.From.Id);
					if (regstat != null)
					{
						regstat.UserRegStatus = 1;
						await db.SaveChangesAsync();
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

			var regStatus = await db.RegistrationStatuses.FirstOrDefaultAsync(stat => stat.TelegramId == userId);
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
				await TakePhoto(bot, update, cts);
			}
			else if(regStatus.UserRegStatus == 6)
			{
				if(update.Message != null)
				{
					var chatid = update.Message.Chat.Id;
					await bot.SendMessage(chatid, "Регистрация завершилась📍", replyMarkup: Keyboards.ReturnFromReg());
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
			if (chatid != null)
			{
				await bot.SendMessage(
					chatId: chatid,
					text: "Введите имя:",
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
		private static async Task TakePhoto(ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken cts)
		{
			long? chatid = TelegramBotUtilities.ReturnChatID(update);
			string? text = "Отправьте фотку для вашего профиля: ";
			if (chatid != null)
			{
				await bot.SendMessage(
					chatId: chatid,
					text: text,
					cancellationToken: cts
					);
			}
		}

		public static async Task<bool> CheckStatus(Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if(update?.Message?.From == null)
				return false;
			long telegramId = update.Message.From.Id;
			return await db.Users.AnyAsync(user => user.TelegramID == telegramId);
		}
		
		#region Answer on profile registration methods
		public static async Task AnswerOnTakeGender(string data, UserProfile? user, Telegram.Bot.Types.Update update, ITelegramBotClient bot, AppDbContext db, UserRegistrationService userregStat)
		{
			if (user != null && update?.CallbackQuery?.Message != null)
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
		}

		public static async Task AnswerOnTakeGroup(ITelegramBotClient bot, string data, Telegram.Bot.Types.Update update, AppDbContext db, UserRegistrationService userregStat, UserProfile user)
		{
			if (update?.CallbackQuery?.Message != null)
			{
				var group = await db.Groups.FirstOrDefaultAsync(grp => grp.Name == data);
				var messageId = update.CallbackQuery.Message.MessageId;
				var chatId = update.CallbackQuery.Message.Chat.Id;
				if (group != null)
				{
					await bot.DeleteMessage(chatId, messageId);
					user.group = group;
					user.GroupID = group.Id;
					userregStat.UserRegStatus = 3;
					await db.SaveChangesAsync();
				}
				await bot.SendMessage(
					chatId,
					$"Вы состоите в группе {data}",
					replyMarkup: Keyboards.ContinueReg()
					);
			}
		}

		public static async Task AnswerOnTakeName(ITelegramBotClient bot, string data, Telegram.Bot.Types.Update update, UserProfile user, AppDbContext db, UserRegistrationService userregStat)
		{
			if(data is  string str && str.Length < 150 && data != null)
			{
				user.Name = data;
				await db.SaveChangesAsync();
				userregStat.UserRegStatus = 4;
				if (update.Message != null)
				{
					await bot.SendMessage(update.Message.Chat.Id, $"Имя в профиле: {data}", replyMarkup: Keyboards.ContinueReg());
				}
			}
		}

		public static async Task AnswerOnTakeLastName(ITelegramBotClient bot, string data, Telegram.Bot.Types.Update update, UserProfile user, AppDbContext db, UserRegistrationService userregStat)
		{
			if (data is string str && str.Length < 150 && data != null)
			{
				user.LastName = data;
				await db.SaveChangesAsync();
				userregStat.UserRegStatus = 5;
				if (update.Message != null)
				{
					await bot.SendMessage(update.Message.Chat.Id, $"Фамилия в профиле: {data}", replyMarkup: Keyboards.ContinueReg());
				}
			}
		}

		public static async Task AnswerOnTakePhoto(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db, UserRegistrationService userregStat)
		{
			if (update.Message?.From != null)
			{
				var user = await db.Users.FirstOrDefaultAsync(u => u.TelegramID == update.Message.From.Id);
				if (user != null && update.Message != null)
				{
					var photoList = update.Message.Photo;
					if (photoList != null)
					{
						var photoId = photoList.Last();
						user.PhotoId = photoId.FileId;
						userregStat.UserRegStatus = 6;
						await bot.SendMessage(update.Message.Chat.Id, "Изображение установлено👌");
						await db.SaveChangesAsync();
					}
				}
			}
		}
		#endregion
		
		#endregion

		#region Modestatus handler
		public static async Task ChangeModeStatus(Telegram.Bot.Types.Update update, AppDbContext db, int status)
		{
			if (update.Message?.From?.Id == null) 
				return;
			if (!await db.ModeServices.AnyAsync(m => m.TelegramId == update.Message.From.Id))
				await db.ModeServices.AddAsync(new ModeService { TelegramId = update.Message.From.Id });

			var userModeStatus = await db.ModeServices.FirstOrDefaultAsync(m => m.TelegramId == update.Message.From.Id);

			if (userModeStatus == null)
				return;
			userModeStatus.ModeStatus = status;
			await db.SaveChangesAsync();
		}
		#endregion
	}
}
