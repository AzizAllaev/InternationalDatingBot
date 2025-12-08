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
using Microsoft.Data.SqlClient;
using System.Xml.Linq;

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
		public DbSet<UserRegistrationService> RegistrationStatuses { get; set; }
		public DbSet<ModeService> ModeServices { get; set; }
		public DbSet<TargetPartnerService> TargetPartnerServices { get; set; }
		public DbSet<Application> Applications { get; set; }
		public DbSet<Attendance> Attendances { get; set; }

		private const string ConnectionStringWithoutDb = @"Server=.\SQLEXPRESS;Trusted_Connection=True;TrustServerCertificate=True;";
		private const string DbName = "InterDating";


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datingbot.db");
			//optionsBuilder.UseSqlite($"Data Source={path}");
			//var connection = Environment.GetEnvironmentVariable("CONNECTION_STRING");
			optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=InterDating;Trusted_Connection=True;TrustServerCertificate=True;");
		}
		public static void EnsureDatabaseCreated()
		{
			// Создаём базу, если её нет
			using (var connection = new SqlConnection(ConnectionStringWithoutDb))
			{
				connection.Open();
				string createDbQuery = $"IF DB_ID('{DbName}') IS NULL CREATE DATABASE {DbName};";
				using (var command = new SqlCommand(createDbQuery, connection))
				{
					command.ExecuteNonQuery();
				}
			}

			// Создаём таблицы через EF
			using (var context = new AppDbContext())
			{
				context.Database.EnsureCreated();
			}
		}
	}
}
