using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClass
{
	public class Pair
	{
		private int Id { get; }
		public int Male {  get; set; }
		public int Female { get; set; }

		public Pair(int id)
		{
			Id = id;
		}
	}
}
