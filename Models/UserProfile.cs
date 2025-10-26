using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class UserProfile
	{
		public int Id { get; set; }
		public long TelegramID { get; set; }
		public string? Username { get; set; }
		public string? Name { get; set; }
		public string? LastName { get; set; }
		public int? Age { get; set; }
		public string? Gender { get; set; }

		public int? GroupID { get; set; }
		public Group? group { get; set; }
	}
}
