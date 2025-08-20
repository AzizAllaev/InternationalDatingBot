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
				var previousPair = FindPairByParticipant(male.Id);
				var previousFemale = females.Single(female => female.Id == previousPair.Female);
				previousFemale.InPair = false;
				DeletePairById(previousPair.Id);
			}
			if (female.InPair)
			{
				var previousPair = FindPairByParticipant(female.Id);
				var previousMale = males.Single(male => male.Id == previousPair.Male);
				previousMale.InPair = false;
				DeletePairById(previousPair.Id);
			}

			male.InPair = true;
			female.InPair = true;

			amountOfPares += 1;
			Pair pair = new Pair(amountOfPares);
			pair.Male = male.Id;
			pair.Female = female.Id;
			pairs.Add(pair);
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
			var a = FindPairByParticipant(pairs, PersonId);
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


		void BreakExistingPair()
	}
}
