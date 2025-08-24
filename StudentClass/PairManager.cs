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

		public void AddPair(Male male, Female female, List<Male> males, List<Female> females)
		{
			if (male.InPair)
			{
				BreakExistingPair(male.Id, males, females);
			}
			if (female.InPair)
			{
				BreakExistingPair(female.Id, males, females);
			}

			male.InPair = true;
			female.InPair = true;

			amountOfPares += 1;
			Pair pair = new Pair(amountOfPares);
			pair.Male = male.Id;
			pair.Female = female.Id;
			pairs.Add(pair);
		}
		public IReadOnlyList<Pair> ReturnPairs()
		{
			return pairs;
		}
		public void DeletePairById(int id)
		{
			var pairToRemove = pairs.FirstOrDefault(p => p.Id == id);
			if (pairToRemove != null)
			{
				pairs.Remove(pairToRemove);
			}
		}
		public void DeletePairByPersonId(int PersonId)
		{
			var a = FindPairByParticipant(PersonId);
			if (a != null)
			{
				DeletePairByObject(a);
			}
		}
		public void DeletePairByObject(Pair pair)
		{
			pairs.Remove(pair);
		}
		public Pair? FindPairByParticipant(int PersonId)
		{
			return pairs.FirstOrDefault(p => p.Male == PersonId || p.Female == PersonId);
		}


		#region
		void BreakExistingPair(int personId, List<Male> males, List<Female> females)
		{
			var pair = FindPairByParticipant(personId);
			if (pair == null)
				return;
			var male = males.FirstOrDefault(p => p.Id == pair.Male);
			var female = females.FirstOrDefault(fem => fem.Id == pair.Female);
			if (male != null)
			{
				male.InPair = false;
			}
			if(female != null)
			{
				female.InPair = false;
			}
			DeletePairByObject(pair);
		}
		#endregion
	}
}
	