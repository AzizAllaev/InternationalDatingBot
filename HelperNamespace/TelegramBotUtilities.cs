using HelperNamespce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;
using Models;
using Microsoft.EntityFrameworkCore;

namespace HelperNamespace
{
	public static class TelegramBotUtilities
	{
		#region Text
		public static async Task<string?> ReturnBaseProfileText(AppDbContext db, Update update)
		{
			if (update?.Message?.From == null)
				return null;

			var user = await db.Users.FirstOrDefaultAsync(u => u.TelegramID == update.Message.From.Id);
			if(user == null) 
				return null;

			var group = await db.Groups.FirstOrDefaultAsync(g => g.Id == user.GroupID);
			if (group == null || user == null)
				return null;

			string text = $"Имя пользователя: {user.Username}\n" +
						$"Группа: {group.Name}\n" +
						$"ФИ профиля: {user.Name} {user.LastName}\n";
			return text;
		}
		public static async Task<string?> ReturnTargetProfileText(UserProfile targetuser, AppDbContext db)
		{
			var group = await db.Groups.FirstOrDefaultAsync(g => g.Id == targetuser.GroupID);
			if(group == null)
				return null;

			string text = $"Имя пользователя: {targetuser.Username}\n" +
						$"Группа: {group.Name}\n" +
						$"ФИ профиля: {targetuser.Name} {targetuser.LastName}\n";
			return text;
		}
		public static string StartRegirstrationText()
		{
			string text = "Начать заполнение профиля";
			return text;
		}
		public static string AskGroupText()
		{
			string text = "Выберите вашу группу: ";
			return text;
		}
		#endregion

		#region User handler methods
		public static long? ReturnChatID(Update update)
		{
			return update?.Message?.Chat?.Id;
		}
		public static string? ReturnNewMessage(Update update)
		{
			if(update?.Message?.Text == null)
				return null;
			return update.Message.Text;
		}
		public static string? ReturnUsername(Update update)
		{
			return update?.Message?.From?.Username;
		}
		#endregion
	}
}
