using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class Attendance
	{
		public int Id { get; set; }
		public long TelegramID { get; set; }
		public string? FullNameAndGroup { get; set; }
		public string? LyceumName { get; set; }
		public string? Status { get; set; }
	}
}
