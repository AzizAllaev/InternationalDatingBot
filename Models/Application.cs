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
		public string? MalePhoneNumber { get; set; }
		public string? MaleTelegramUser { get; set; }
		public string? MaleLyceumName { get; set; }
		public int? MaleGroupId { get; set; }
		public Group? mgroup { get; set; }
		public string? MalePurpose { get; set; }

		public string? FemaleFullName { get; set; }
		public string? FemaleTelegramUser { get; set; }
		public string? FemaleLyceumName { get; set; }
		public int? FemaleGroupId { get; set; }
		public Group? fgroup { get; set; }
		public string? FemalePurpose { get; set; }
	}
}
