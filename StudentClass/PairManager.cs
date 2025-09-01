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

		public bool IsInPair(Person person)
		{
			var pair = FindPairByParticipant(person.Id);
			if(pair != null)
			{
				if(pair.Male != null && pair.Female != null)
				{
					return true;
				}
				else 
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
		public void AddPair(Male male, Female female, List<Male> males, List<Female> females)
		{
			if (IsInPair(male))
			{
				DeletePairByPersonId(male.Id);
			}
			if (IsInPair(female))
			{
				DeletePairByPersonId(female.Id);
			}


			amountOfPares += 1;
			Pair pair = new Pair(amountOfPares);
			pair.Male = male;
			pair.Female = female;
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
			return pairs.FirstOrDefault(p => p.Female.Id == PersonId || p.Male.Id == PersonId);
		}

	}
}
	