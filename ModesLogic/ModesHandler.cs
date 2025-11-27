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
			if (!chatID.HasValue)
				return;

			var keyboard = Keyboards.ReturnFromProfile();
			string? username = TelegramBotUtilities.ReturnUsername(update);
			string? messageForButton = await TelegramBotUtilities.ReturnBaseProfileText(update, db);
			var user = await db.Users.FirstOrDefaultAsync(x => x.TelegramID == update.Message.From.Id);
			if (user == null)
			{
				await bot.SendMessage(chatID, "Профиль не заполнен", replyMarkup: Keyboards.ReturnFromProfile());
				return;
			}

			await UsernameController(update, db, bot, user);

			if (messageForButton != null && user != null && user.PhotoId != null && user.Name != null)
			{
				await bot.SendPhoto(
					chatId: chatID,
					caption: messageForButton,
					replyMarkup: keyboard,
					cancellationToken: clt,
					photo: user.PhotoId
					);
			}
		}
		#endregion

		#region Delete user from DB
		public static async Task DeleteUser(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if(update?.Message?.From == null) 
				return;

			var user = await db.Users.FirstOrDefaultAsync(u => update.Message.From.Id == u.TelegramID);
			if (user == null)
			{
				await bot.SendMessage(update.Message.From.Id, $"Ваших данных в базе нет🤔");
				return;
			}
			//if (user.Gender == "Female")
			//{
			//	var likes = await db.Likes.Where(l => l.FemaleId == user.Id).ToListAsync();
			//	var target = await db.TargetPartnerServices.FirstOrDefaultAsync(t => t.TelegramID == update.Message.From.Id);
			//	var 
			//}

			db.Remove(user);
			await db.SaveChangesAsync();
			await bot.SendMessage(update.Message.From.Id, "Ваши данные были удалены из базы😉");
		}
		#endregion

		#region Select partner menu

		#region Partner showcase
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
		#endregion

		#region method to find pair
		public static async Task FindPair(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;
			var baseuser = await GetBaseuser(bot, update, db);
			var targetuser = await GetTargetuser(bot, update, db);
			var target = await db.TargetPartnerServices.FirstOrDefaultAsync(t => t.TelegramID == update.Message.From.Id);
			if (baseuser == null)
				return;

			if (target == null)
			{
				if (!await db.TargetPartnerServices.AnyAsync(t => t.TelegramID == update.Message.From.Id))
				{
					await db.TargetPartnerServices.AddAsync(new TargetPartnerService { TelegramID = update.Message.From.Id, LastUserId = 0 });
					await db.SaveChangesAsync();
					await bot.SendMessage(baseuser.TelegramID, "Попроубйте ещё раз");
					return;
				}
				return;
			}
			
			if(targetuser == null)
			{

				if (await db.Users.AnyAsync(u => u.Id > 0 && u.Gender != baseuser.Gender))
				{
					target.LastUserId = 0;
					await db.SaveChangesAsync();
					await bot.SendMessage(baseuser.TelegramID, "Вы пролистали всех пользователей");
				}
				else
				{
					target.LastUserId = 0;
					await db.SaveChangesAsync();
					await bot.SendMessage(baseuser.TelegramID, "Пользователей нет😒", replyMarkup: Keyboards.MakeMainMenuKeyboard());
				}
				return;
			}
			await SendTargetUserProfile(bot, update, db, targetuser, target);

			target.LastUserId = targetuser.Id;
			await db.SaveChangesAsync();
		}

		public static async Task SendTargetUserProfile(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db, UserProfile targetuser, TargetPartnerService target)
		{
			if (update?.Message?.From == null)
				return;

			if (targetuser.PhotoId == null || targetuser.Name == null || targetuser.GroupID == null || targetuser.LastName == null || targetuser.Username == null)
			{
				target.LastUserId = targetuser.Id;
				await db.SaveChangesAsync();
				await bot.SendMessage(targetuser.TelegramID, "К сожалению пользователи не видят вашего профиля, так как у вас отсутсвует юзер либо не заполнен профиль");
				await bot.SendMessage(update.Message.From.Id, "Попробуйте ещё раз");
				return;
			}
			Console.WriteLine($"Target: {targetuser.Name} || {targetuser.Username}");
			await bot.SendPhoto(
				chatId: update.Message.Chat.Id,
				caption: await TelegramBotUtilities.ReturnTargetProfileText(targetuser, db),
				replyMarkup: Keyboards.LikeDislikeButtons(),
				photo: targetuser.PhotoId
				);
		}

		public static async Task SendTargetUserProfileWithId(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db, UserProfile targetuser, TargetPartnerService target)
		{
			if (update?.Message?.From == null)
				return;

			if (targetuser.PhotoId == null || targetuser.Name == null || targetuser.GroupID == null || targetuser.LastName == null || targetuser.Username == null)
			{
				target.LastUserId = targetuser.Id;
				await db.SaveChangesAsync();
				await bot.SendMessage(targetuser.TelegramID, "К сожалению пользователи не видят вашего профиля, так как у вас отсутсвует юзер либо не заполнен профиль");
				await bot.SendMessage(update.Message.From.Id, "Попробуйте ещё раз");
				return;
			}
			Console.WriteLine($"Target: {targetuser.Name} || {targetuser.Username}");
			await bot.SendPhoto(
				chatId: update.Message.Chat.Id,
				caption: await TelegramBotUtilities.ReturnTargetProfileTextWithId(targetuser, db),
				replyMarkup: Keyboards.LikeDislikeButtons(),
				photo: targetuser.PhotoId
				);
		}
		#endregion

		#region Handle like/dislike
		public static async Task HandleLike(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			var target = await db.TargetPartnerServices.FirstOrDefaultAsync(t => t.TelegramID == update.Message.From.Id);
			if (target == null)
				return;
			var baseuser = await GetBaseuser(bot, update, db);
			var targetuser = await db.Users.FirstOrDefaultAsync(u => u.Id == target.LastUserId);
			if (baseuser == null)
			{
				Console.WriteLine("baseuser == null");
				return;
			}
			if(targetuser == null)
			{
				Console.WriteLine("targetuser == null");
				return;
			}
			await ProcessLike(bot, update, db, targetuser, baseuser);

			await db.SaveChangesAsync();
			await FindPair(bot, update, db);
		}
		public static async Task ProcessLike(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db, UserProfile targetuser, UserProfile baseuser)
		{
			if (baseuser.Gender == "Male")
			{
				if (!await db.Likes.AnyAsync(l => l.MaleId == baseuser.Id && l.FemaleId == targetuser.Id))
				{
					await db.Likes.AddAsync(new Like { MaleId = baseuser.Id, FemaleId = targetuser.Id, SenderId = baseuser.Id });
					await bot.SendMessage(baseuser.TelegramID, $"Вы лайкнули {targetuser.Name}");
					await bot.SendMessage(targetuser.TelegramID, "Вас лайкнули");
				}
				else if (await db.Likes.AnyAsync(l => l.FemaleId == targetuser.Id && l.MaleId == baseuser.Id && l.SenderId == targetuser.Id))
				{
					await bot.SendMessage(baseuser.TelegramID, $"У вас взаимный лайк с @{targetuser.Username}");
					await bot.SendMessage(targetuser.TelegramID, $"У вас взаимный лайк с @{baseuser.Username}");
					await DeleteLike(bot, update, db, targetuser);
				}
			}
			else if (baseuser.Gender == "Female")
			{
				if (!await db.Likes.AnyAsync(l => l.FemaleId == baseuser.Id && l.MaleId == targetuser.Id))
				{
					await db.Likes.AddAsync(new Like { MaleId = targetuser.Id, FemaleId = baseuser.Id, SenderId = baseuser.Id });
					await bot.SendMessage(baseuser.TelegramID, $"Вы лайкнули {targetuser.Name}");
					await bot.SendMessage(targetuser.TelegramID, "Вас лайкнули");
				}
				else if (await db.Likes.AnyAsync(l => l.FemaleId == baseuser.Id && l.MaleId == targetuser.Id && l.SenderId == targetuser.Id))
				{
					await bot.SendMessage(baseuser.TelegramID, $"У вас взаимный лайк с @{targetuser.Username}");
					await bot.SendMessage(targetuser.TelegramID, $"У вас взаимный лайк с @{baseuser.Username}");
					await DeleteLike(bot, update, db, targetuser);
				}
			}
		}
		public static async Task DeleteLike(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db, UserProfile targetuser)
		{
			if(update?.Message?.From == null)
				return;
			
			var baseuser = await db.Users.FirstOrDefaultAsync(u => u.TelegramID == update.Message.From.Id);
			if (baseuser == null) 
				return;

			var like = await db.Likes.FirstOrDefaultAsync(l => l.FemaleId == baseuser.Id && l.MaleId == targetuser.Id && l.SenderId == targetuser.Id);
			if (like == null) 
				return;

			db.Likes.Entry(like).State = EntityState.Deleted;
		}

		public static async Task HandleDislike(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			await FindPair(bot, update, db);
		}

		public static async Task MatchUser(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			var baseuser = await db.Users.FirstOrDefaultAsync(u => u.TelegramID == update.Message.From.Id);
			string? MatchId = TelegramBotUtilities.ReturnNewMessage(update);
			int? targetuserId = Convert.ToInt32(MatchId);
			if (targetuserId == null || baseuser == null)
				return;

			var targetuser = await db.Users.FirstOrDefaultAsync(u => u.Id == targetuserId);
			if (targetuser == null) 
				return;

			if (baseuser.Gender == "Female")
			{
				if(await db.Likes.AnyAsync(l => l.FemaleId == baseuser.Id && l.MaleId == targetuserId))
				{
					await bot.SendMessage(baseuser.TelegramID, $"У вас взаимный лайк с @{targetuser.Username}");
					await bot.SendMessage(targetuser.TelegramID, $"У вас взаимный лайк с @{baseuser.Username}");

					var like = await db.Likes.FirstOrDefaultAsync(l => l.FemaleId == baseuser.Id && l.MaleId == targetuser.Id);
					if (like == null)
						return;
					db.Likes.Remove(like);
					await db.SaveChangesAsync();
				}
				else
				{
					await bot.SendMessage(baseuser.TelegramID, "Неправильный ID");
				}
			}
			else if(baseuser.Gender == "Male")
			{
				if(await db.Likes.AnyAsync(l => l.MaleId == baseuser.Id && l.FemaleId == targetuserId))
				{
					await bot.SendMessage(baseuser.TelegramID, $"У вас взаимный лайк с @{targetuser.Username}");
					await bot.SendMessage(targetuser.TelegramID, $"У вас взаимный лайк с @{baseuser.Username}");

					var like = await db.Likes.FirstOrDefaultAsync(l => l.FemaleId == targetuser.Id && l.MaleId == baseuser.Id);
					if (like == null)
						return;
					db.Likes.Remove(like);
					await db.SaveChangesAsync();
				}
				else
				{
					await bot.SendMessage(baseuser.TelegramID, "Неправильный ID");	
				}
			}
			else
			{
				return;
			}
		}
		#endregion

		#region View all likes
		public static async Task ViewLikes(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			var baseuser = await GetBaseuser(bot, update, db);
			if (baseuser == null)
				return;
			
			List<int> ?userIds;

			if (baseuser.Gender == "Male")
			{
				userIds = await db.Likes
					.Where(l => l.MaleId == baseuser.Id && l.FemaleId.HasValue && l.SenderId != baseuser.Id)
					.Select(l => l.FemaleId!.Value)
					.ToListAsync();
			}
			else if(baseuser.Gender == "Female")
			{
				userIds = await db.Likes
					.Where(l => l.FemaleId == baseuser.Id && l.MaleId.HasValue && l.SenderId != baseuser.Id)
					.Select(l => l.MaleId!.Value)
					.ToListAsync();
			}
			else
			{
				userIds = null;
			}
			
			if (userIds == null || userIds.Count == 0)
			{
				await bot.SendMessage(baseuser.TelegramID, "Пока никто не лайкал ваш профиль.");
				return;
			}

			var users = await db.Users
				.Include(u => u.group)
				.Where(u => userIds.Contains(u.Id))
				.ToListAsync();

			var sb = new System.Text.StringBuilder();
			var target = await db.TargetPartnerServices.FirstOrDefaultAsync(t => t.TelegramID == update.Message.From.Id);
			if (target == null)
				return;
			Console.WriteLine("test");
			foreach (var user in users)
			{
				var groupName = user.group?.Name ?? "Без группы";
				await SendTargetUserProfileWithId(bot, update, db, user, target);
			}
			await bot.SendMessage(baseuser.TelegramID, "Впишите Id пользователя приглашение которого вы принимаете.");
		}
		#endregion

		#region Methods to find user
		public static async Task<UserProfile?> GetBaseuser(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return null;

			var baseuser = await db.Users.FirstOrDefaultAsync(u => u.TelegramID == update.Message.From.Id);
			if (baseuser == null)
				return null;

			return baseuser;
		}
		public static async Task<UserProfile?> GetTargetuser(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return null;
			var target = await db.TargetPartnerServices.FirstOrDefaultAsync(t => t.TelegramID == update.Message.From.Id);
			var baseuser = await db.Users.FirstOrDefaultAsync(u => u.TelegramID == update.Message.From.Id);
			if (baseuser == null || target == null)
				return null;
			
			var targetuser = await db.Users.FirstOrDefaultAsync(u => u.Id > target.LastUserId && u.Gender != baseuser.Gender);
			if (targetuser == null)
				return null;
			Console.WriteLine($"{targetuser.Name} {targetuser.LastName} {targetuser.Id} {targetuser.TelegramID}");
			return targetuser;
		}
		public static async Task HandleReverse(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			var target = await db.TargetPartnerServices.FirstOrDefaultAsync(t => t.TelegramID == update.Message.From.Id);
			if (target == null)
				return;

			if(target.LastUserId > 0)
				target.LastUserId -= 1;
			
			await db.SaveChangesAsync();
		}
		#endregion

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

		#region Other
		public static async Task ChangeModeStatus(Telegram.Bot.Types.Update update, AppDbContext db, int status)
		{
			if (update.Message?.From == null)
				return;
			if (!await db.ModeServices.AnyAsync(m => m.TelegramId == update.Message.From.Id))
			{
				await db.ModeServices.AddAsync(new ModeService { TelegramId = update.Message.From.Id });
				await db.SaveChangesAsync();
			}

			var userModeStatus = await db.ModeServices.FirstOrDefaultAsync(m => m.TelegramId == update.Message.From.Id);

			if (userModeStatus == null)
				return;
			userModeStatus.ModeStatus = status;
			await db.SaveChangesAsync();
		}
		public static async Task<int?> ReturnModeStatus(Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update.Message?.From == null)
				return null;

			var status = await db.ModeServices.FirstOrDefaultAsync(s => s.TelegramId == update.Message.From.Id);
			if (status == null)
				return null;
			
			return status.ModeStatus;
		}
		private static async Task UsernameController(Telegram.Bot.Types.Update update, AppDbContext db, ITelegramBotClient bot, UserProfile user)
		{
			if (update?.Message?.From == null)
				return;

			bool? test = TelegramBotUtilities.CheckActualUsername(update, user);

			switch (test)
			{
				case null:
					await bot.SendMessage(update.Message.From.Id, "Поставьте имя пользователя для корректной работы бота, и для отобрежения другим пользователям❎");
					break;
				case false:
					user.Username = update.Message.From.Username;
					await db.SaveChangesAsync();
					break;
			}
		}
		#endregion
	}
}
