using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class Application
	{
		public int Id { get; set; }
		public long TelegramID { get; set; }

		public string? MaleFullName { get; set; }
		public string? MaleTelegramUserAndPhoneNumber { get; set; }
		public string? MaleLyceumName { get; set; }

		public string? PurposeOfMeeting { get; set; }

		public string? FemaleFullName { get; set; }
		public string? FemaleTelegramUserAndPhoneNumber { get; set; }
		public string? FemaleLyceumName { get; set; }
	}
}
