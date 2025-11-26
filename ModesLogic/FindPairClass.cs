using HelperNamespace;
using HelperNamespce;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace ModesLogic
{
	public class FindPairClass
	{
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
			if (target == null)
			{
				if (!await db.TargetPartnerServices.AnyAsync(t => t.TelegramID == update.Message.From.Id))
				{
					await db.TargetPartnerServices.AddAsync(new TargetPartnerService { TelegramID = update.Message.From.Id, LastUserId = 0 });
					await db.SaveChangesAsync();
					await FindPair(bot, update, db);
					return;
				}
				return;
			}
			if (baseuser == null)
				return;

			if (targetuser == null)
			{

				if (await db.Users.AnyAsync(u => u.Id > 0 && u.Gender != baseuser.Gender))
				{
					target.LastUserId = 0;
					await db.SaveChangesAsync();
					await FindPair(bot, update, db);
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
			if (targetuser.PhotoId == null || targetuser.Name == null || targetuser.GroupID == null || targetuser.LastName == null || targetuser.Username == null)
			{
				target.LastUserId = targetuser.Id;
				await db.SaveChangesAsync();
				await bot.SendMessage(targetuser.TelegramID, "К сожалению пользователи не видят вашего профиля, так как у вас отсутсвует юзер либо не заполнен профиль");
				await FindPair(bot, update, db);
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
			if (targetuser == null)
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
			if (update?.Message?.From == null)
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
		#endregion

		#region View all likes
		public static async Task ViewLikes(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return;

			var baseuser = await GetBaseuser(bot, update, db);
			if (baseuser == null)
				return;

			List<int>? userIds;

			if (baseuser.Gender == "Male")
			{
				userIds = await db.Likes
					.Where(l => l.MaleId == baseuser.Id && l.FemaleId.HasValue && l.SenderId != baseuser.Id)
					.Select(l => l.FemaleId!.Value)
					.ToListAsync();
			}
			else if (baseuser.Gender == "Female")
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

			foreach (var user in users)
			{
				var groupName = user.group?.Name ?? "Без группы";
				sb.AppendLine($"ID: {user.Id} \n ФИ: {user.Name} {user.LastName} \n Группа: {groupName} \n Юзер: @{user.Username}");
			}

			await bot.SendMessage(baseuser.TelegramID, sb.ToString());
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

			if (target.LastUserId > 0)
				target.LastUserId -= 1;

			await db.SaveChangesAsync();
		}
		#endregion

		#endregion
	}
}
