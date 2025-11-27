using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class UserRegistrationService
	{
		public int Id { get; set; }
		public long TelegramId { get; set; }
		public UserProfile? user { get; set; }

		public int? UserRegStatus { get; set; }
		public int? AppStatus { get; set; }
	}
}
