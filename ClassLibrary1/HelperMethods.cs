using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace HelperNamespace
{
	public static class HelperMethods
	{
		public static string ReturnNewMessage(Update update)
		{
			if (update.Message != null)
			{
					return update.Message.Text;
			}
			else
			{
				throw new ArgumentNullException("Сообщение не содержит текста");
			}
		}
		public static string ReturnUsername(Update update)
		{
			var user = update.Message.From;
			string username = user.Username;
			return username;
		}
	}
}
