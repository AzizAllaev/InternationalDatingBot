using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class User
	{
		public int Id { get; set; }
		public long TelegramID { get; set; }
		public string? Username { get; set; }
		public int Age { get; set; }
		public string? Gender { get; set; }


		public Group? group { get; set; }
	}
}
