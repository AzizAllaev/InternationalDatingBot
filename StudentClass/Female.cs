namespace StudentClass
{
	public class Male : Person 
	{
		private List<Female> _topChoises = new();

		public Person ReturnTopStudent()
		{
			return _topChoises[0];
		}
		public IReadOnlyList<Person> GetStudents()
		{
			return _topChoises;
		}
		public void AddToChoises(Female person)
		{
			if (!_topChoises.Contains(person))
				if (_topChoises.Count < 6)
					_topChoises.Add(person);
		}

	}
	public class Female : Person 
	{
		private List<Male> _topChoises = new();
		public Person ReturnTopStudent()
		{
			return _topChoises[0];
		}
		public IReadOnlyList<Person> GetStudents()
		{
			return _topChoises;
		}
		public void AddToChoises(Male person)
		{
			if (!_topChoises.Contains(person))
				if (_topChoises.Count < 6)
					_topChoises.Add(person);
		}

	}
}
