using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Models
{
	public class AppDbContext : DbContext
	{
		public DbSet<UserProfile> Users { get; set; }
		public DbSet<Like> Likes { get; set; }
		public DbSet<Group> Groups { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=InterDating;Trusted_Connection=True;TrustServerCertificate=True;");
		}
	}
	#region Methods with DB
	public class DbControls
	{
		public static void CheckStatus(ITelegramBotClient bot, Telegram.Bot.Types.Update update, AppDbContext db)
		{
			if (update?.Message?.From != null)
			{
				long userId = update.Message.From.Id;

				if (!db.Users.Any(user => user.TelegramID == userId))
				{
					db.Users.Add(new UserProfile { TelegramID = userId});
				}
			}
		}
	}
	#endregion
}
