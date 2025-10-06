using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Handlers;
using Models;

namespace InternationalDating
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var bot = new TelegramBotClient("7709122756:AAHs1xvGGrShZ3U0MrJTKMwVsWGjJCfgdls");
			using var cts = new CancellationTokenSource();

			bot.StartReceiving(
				ClassHandlers.HandleUpdateAsync,
				ClassHandlers.HandleError, 
				new Telegram.Bot.Polling.ReceiverOptions(), 
				cts.Token);

			Console.ReadLine();
		}

	}

	public class AppDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Like> Likes { get; set; }
		public DbSet<Group> Groups { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=InterDating;Trusted_Connection=True;TrustServerCertificate=True;");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>()
				.HasMany(user => user.group)
				.HasOne(g => g.users)
		}
	}
}
