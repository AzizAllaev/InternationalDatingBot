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
		public static async Task<string?> ReturnBaseProfileText(Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return null;

			var user = await db.Users.FirstOrDefaultAsync(u => u.TelegramID == update.Message.From.Id);
			if(user == null) 
				return null;

			var group = await db.Groups.FirstOrDefaultAsync(g => g.Id == user.GroupID);
			if (group == null || user == null)
				return null;

			string text = $"Имя пользователя: {update.Message.From.Username}\n" +
						$"Группа: {group.Name}\n" +
						$"ФИ профиля: {user.Name} {user.LastName}\n" +
						$"" +
						$"!!!  Предупреждение, Если у вас нету юзера вы не будете отображаться другим пользователям. Если вы поменяли юзер то зайдите и проверьте его тут, изменился ли он. Иначе в случае взаимного лайка ваш юзер будет отображаться не корректно !!!";
			return text;
		}
		public static async Task<string?> ReturnTargetProfileText(UserProfile targetuser, AppDbContext db)
		{
			var group = await db.Groups.FirstOrDefaultAsync(g => g.Id == targetuser.GroupID);
			if(group == null)
				return null;

			string 	text = $"Группа: {group.Name}\n" +
						$"ФИ профиля: {targetuser.Name} {targetuser.LastName}\n";
			return text;
		}
		public static async Task<string?> ReturnTargetProfileTextWithId(UserProfile targetuser, AppDbContext db)
		{
			var group = await db.Groups.FirstOrDefaultAsync(g => g.Id == targetuser.GroupID);
			if (group == null)
				return null;

			string text = $"Группа: {group.Name}\n" +
						$"ФИ профиля: {targetuser.Name} {targetuser.LastName}\n" +
						$"Id: {targetuser.Id}";
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

		public static string ReturnMDataText()
		{
			string text;
			text = "Укажите ФИО парня и группу, в таком виде:\n" +
				"<b>Фамилия Имя Отчество Группа</b>";
			return text;
		}
		public static string ReturnMPhoneNumberText()
		{
			string text = "Введите номер телефона парня в таком виде: \n" +
				"<b>+998 XX XXX XX XX</b>";

			return text;
		}

		public static string ReturnFDataText()
		{
			string text;
			text = "Укажите ФИО девушки и группу в таком виде:\n" +
				"<b>Фамилия Имя Отчество Группа</b>";
			return text;
		}
		public static string ReturnFPhoneNumberText()
		{
			string text = "Введите номер телефона девушки в таком виде: \n" +
				"<b>+998 XX XXX XX XX</b>";

			return text;
		}

		public static string StudentsWarning()
		{
			string text = "⚠️Убедительная просьба, старайтесь вводить данные правильно. Перед отправкой проверьте и перечитайте заявку и после этого отправляйте её. Вначале заполняются данные парня, после чего девушки. В случае неправильных данных заявка будет отклонена⚠️";

			return text;
		}
		public static string StudentsWarningatAttendance()
		{
			string text = "Убедительная просьба вводить данные корректно, начиная от имени заканчивая группой. Вводите группу в таком виде 1ВТН1, 2ЭК1 ровно так как они пишутся в документах";

			return text;
		}

		public static string ReturnApplicationConfirmationText(Models.Application app)
		{
			string text = $"ФИО парня: {app.MaleFullName}\n" +
				$"ФИО девушки: {app.FemaleFullName}\n" +
				$"Прочитайте внимательно есть ли ошибки";

			return text;
		}
		public static string ReturnFullNameConfirmationText(string data)
		{
			string text = $"Подтвердите ФИО и группу: \n" +
				$"<b>{data}</b>";
			return text;
		}
		public static string ReturnPhoneNumberAndUsernameConfirmText(string data)
		{
			string text = $"Подтвердите номер телефона: \n" +
				$"<b>{data}</b>";
			
			return text;
		}
		public static string ReturnPurposeConfirmText(string data)
		{
			string text = $"Подтвердите: \n" +
				$"<i>{data}</i>";
			return text;
		}

		public static string ReturnAttendanceText()
		{
			string text;
			text = "Укажите ФИО и группу в таком виде:\n" +
				"<b>Иван Иванович Иванов 1ТН1</b>";
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

		#region Check user profile data
		public static async Task<bool> CheckProfileFill(Update update, AppDbContext db)
		{
			if (update?.Message?.From == null)
				return false;

			var user = await db.Users.FirstOrDefaultAsync(u => u.TelegramID == update.Message.From.Id);
			if (user == null)
				return false;

			if (user.PhotoId == null || user.Name == null || user.LastName == null || user.GroupID == null)
				return false;
			else
				return true;
		}
		public static bool CheckProfileFillByUser(Update update, UserProfile user)
		{
			if (user.PhotoId == null || user.Name == null || user.LastName == null || user.GroupID == null)
				return false;

			return true;
		}
		public static bool? CheckActualUsername(Update update, UserProfile user)
		{
			if (update?.Message?.From == null)
				return false;

			if (update.Message.From.Username == null)
				return null;

			if (user.Username == update.Message.From.Username)
			{
				return true;
			}
			else if (user.Username != update.Message.From.Username)
			{
				return false;
			}
			else
			{
				return null;
			}
		}
		#endregion
	}
}
