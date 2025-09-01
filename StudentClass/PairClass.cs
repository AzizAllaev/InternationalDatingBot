using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClass
{
	public class Pair
	{
		public int Id { get; }
		public Male? Male {  get; set; }
		public Female? Female { get; set; }

		public Pair(int id)
		{
			Id = id;
		}
	}
}
