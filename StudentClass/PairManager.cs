using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClass
{
	public class PairManager
	{
		private List<Pair> pairs = new();
		private int amountOfPares;

		public void AddPair(Male male, Female female)
		{
			amountOfPares += 1;
			Pair pair = new Pair(amountOfPares);
			pair.Male = male.Id;
			pair.Female = female.Id;
		} 
	}
}
