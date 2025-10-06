using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class Like
	{
		public int Id { get; set; }

		public User? Male { get; set; }
		public User? Female { get; set; }
	}
}
