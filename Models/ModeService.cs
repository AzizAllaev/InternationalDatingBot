using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	/// <summary>
	/// This class show in which mode user.
	/// For example:
	/// 1 - Main menu
	/// 2 - 
	/// </summary>
	public class ModeService
	{
		public int Id { get; set; }
		public long? TelegramId { get; set; }
		public int ModeStatus { get; set; }
	}
}
