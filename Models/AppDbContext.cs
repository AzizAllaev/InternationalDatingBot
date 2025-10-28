using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Telegram.Bot;
using Telegram.Bot.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Models
{
	/// <summary>
	/// Migration command is:
	/// Add-Migration MigrationName -StartupProject InternationalDating -Project Models
	/// Update command is:
	/// Update-Database -StartupProject InternationalDating -Project Models
	/// </summary>

	public class AppDbContext : DbContext
	{
		public DbSet<UserProfile> Users { get; set; }
		public DbSet<Like> Likes { get; set; }
		public DbSet<Group> Groups { get; set; }
		public DbSet<UserRegistrationStatus> RegistrationStatuses { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//var connection = Environment.GetEnvironmentVariable("CONNECTION_STRING");
			optionsBuilder.UseSqlite("Data Source=Data/bot.db");
			//optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=InterDating;Trusted_Connection=True;TrustServerCertificate=True;");
		}
	}
}
