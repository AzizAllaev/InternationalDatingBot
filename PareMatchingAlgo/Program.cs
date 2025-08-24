using StudentClass;

namespace PareMatchingAlgo
{
	internal class Program
	{
		static void Main(string[] args)
		{
			// Сами пользователи
			List<Female> females = new List<Female>();
			List<Male> males = new List<Male>();
			PairManager manager = new PairManager();

			while (true)
			{
				Console.WriteLine("1 Add male || 2 Add female || 3 Add to choises || 4 Show choises || 5 Show result");
				ConsoleKeyInfo mess = Console.ReadKey(intercept:true);
				switch(mess.Key)
				{
					case ConsoleKey.D1:
						Console.Clear();
						AddNewM(males);
						Console.Clear();
						break;
					case ConsoleKey.D2:
						Console.Clear();
						AddNewG(females);
						Console.Clear();
						break;
					case ConsoleKey.D3:
						Console.Clear();
						AddTopChoises(females, males);
						Console.Clear();
						break;
					case ConsoleKey.D4:
						Console.Clear();
						ShowChoises(females, males);
						Console.Clear();
						break;
					case ConsoleKey.D5:
						Console.Clear();
						RunAlgoritm(females, males, manager);
						ShowPairs(manager, males, females);
						Console.Clear();
						break;
				}
			}
		}

		static void AddTopChoises(List<Female> females, List<Male> males)
		{
			Console.WriteLine("1 - Male || 2 - Female");
			ConsoleKeyInfo mess0 = Console.ReadKey();
			if (mess0.Key == ConsoleKey.D2)
			{
				ChoiseHandlerFemale(females, males);
			}
			else if(mess0.Key == ConsoleKey.D1)
			{
				ChoiseHandlerMale(females, males);
			}
		}

		#region Algoritm to make pairs

		static void ShowPairs(PairManager manager, List<Male> males, List<Female> females)
		{ 
			var ListOfPair = manager.ReturnPairs();
			int i = 0;
			foreach (var pair in ListOfPair)
			{
				i++;
				var male = males.FirstOrDefault(mal => mal.Id == pair.Male);
				var female = females.FirstOrDefault(femal => femal.Id == pair.Female);
				if (male != null && female != null) 
					Console.Write($"{male.Name} <=> {female.Name}");
			}
			Console.ReadLine();
		}

		static void RunAlgoritm(List<Female> females, List<Male> males, PairManager manager)
		{ 
			Queue<Male> maleQueue = new Queue<Male>(males);

			while (maleQueue.Count > 0)
			{
				var selectedmale = maleQueue.Dequeue();

				if (!selectedmale.InPair)
				{
					var TopChoiseOfMale = selectedmale.GetStudents();
					var queueListOfChoises = new Queue<Female>(TopChoiseOfMale);
					while (queueListOfChoises.Count > 0)
					{
						var actualfemale = queueListOfChoises.Dequeue();
						if (actualfemale.InPair)
						{
							var toplistOfFemale = actualfemale.GetStudents();
							if (toplistOfFemale.Contains(selectedmale))
							{
								var PairOfFemale = manager.FindPairByParticipant(actualfemale.Id);
								int positionOfPairedMale = toplistOfFemale.ToList().IndexOf(males.Single(male => male.Id == PairOfFemale.Male));
								int positionOfactualmale = toplistOfFemale.ToList().IndexOf(selectedmale);
								if(positionOfPairedMale > positionOfactualmale)
								{
									manager.AddPair(selectedmale, actualfemale, males, females);
									break;
								}
							}
						}
						if (!actualfemale.InPair)
						{
							manager.AddPair(selectedmale, actualfemale, males, females);
							break;
						}
					}
				}
			}
		}
		#endregion

		#region Methods to show choises
		static void ShowChoises(List<Female> females, List<Male> males)
		{
			bool stop = false;
			for (int i = 0; i <= males.Count - 1; i++)
			{
				var male = males[i];
				var ListOfChoises = male.GetStudents();
				Console.Write($"{male.Name}: ");
				for(int j = 0; j <= ListOfChoises.Count - 1 ;j++)
				{
					var female = ListOfChoises[j];
					if(j == ListOfChoises.Count - 1) 
					{
						Console.WriteLine($"{female.Name}");
					}
					else 
					{
						Console.Write($"{female.Name} => ");
					}
				}
			}
			Console.ReadLine();
		}
		#endregion

		#region Female

		static void ChoiseHandlerFemale(List<Female> females, List<Male> males)
		{
			try
			{
				Console.Clear();
				var a = ReturnFemaleUser(females);

				Console.WriteLine($"User ID: {a.Id} || Name: {a.Name} || Now you can add choises to this user");
				Console.WriteLine("To add user press 1");
				ConsoleKeyInfo mess1 = Console.ReadKey();
				if (mess1.Key == ConsoleKey.D1)
					ChoiseAddFemale(males, a);
			}
			catch (Exception ex) { Console.WriteLine(ex.Message); }
		}

		static void ChoiseAddFemale(List<Male> males, Female person)
		{
			try
			{
				Console.Clear();
				bool b = true;
				while (b)
				{
					var a = ReturnMaleUser(males);
					Console.WriteLine($"Add user {a.Name} || To add press Enter... || To cancel press 1");
					ConsoleKeyInfo keyInfo = Console.ReadKey();
					switch (keyInfo.Key)
					{
						case ConsoleKey.Enter:
							person.AddToChoises(a);
							break;
						case ConsoleKey.D1:
							b = false;
							break;
					}
					Console.WriteLine("User successfully added to top, to exti press 1");
					ConsoleKeyInfo keyInfo1 = Console.ReadKey();
					if (keyInfo1.Key == ConsoleKey.D1)
						b = false;
				}
			}
			catch (Exception ex) { Console.WriteLine(ex.Message); }
		}
			#endregion

		#region Male
			static void ChoiseHandlerMale(List<Female> females, List<Male> males)
		{
			Console.Clear();
			var a = ReturnMaleUser(males);

			Console.WriteLine($"User ID: {a.Id} || Name: {a.Name} || Now you can add choises to this user");
			Console.WriteLine("To add user press 1");
			ConsoleKeyInfo mess1 = Console.ReadKey();
			if (mess1.Key == ConsoleKey.D1)
				ChoiseAddMale(females, a);
		}
		static void ChoiseAddMale(List<Female> females, Male person)
		{
			Console.Clear();
			bool b = true;
			while (b)
			{
				var a = ReturnFemaleUser(females);
				Console.WriteLine($"Add user {a.Name} || To add press Enter... || To cancel press 1");
				ConsoleKeyInfo keyInfo = Console.ReadKey();
				switch (keyInfo.Key)
				{
					case ConsoleKey.Enter:
						person.AddToChoises(a);
						break;
					case ConsoleKey.D1:
						b = false;
						break;
				}
				Console.WriteLine("User successfully added to top, to exti press 1");
				ConsoleKeyInfo keyInfo1 = Console.ReadKey();
				if (keyInfo1.Key == ConsoleKey.D1)
					b = false;
			}
		}
		#endregion

		#region Methods to find users
		static Female ReturnFemaleUser(List<Female> female)
		{
			Console.Write("Write ID: ");
			int Id = Convert.ToInt32(Console.ReadLine());
			return female.Single(p => p.Id == Id);
		}
		static Male ReturnMaleUser(List<Male> male)
		{
			Console.Write("Write ID: ");
			int Id = Convert.ToInt32(Console.ReadLine());

			return male.Single(p => p.Id == Id);
		}
		#endregion

		#region Entity adders
		public static void AddNewM(List<Male> male)
		{
			Console.Write("Введите ID: ");
			try
			{
				int id = Convert.ToInt32(Console.ReadLine());
				Console.Write("Введите имя: ");
				string name = Console.ReadLine();
				Male m1 = new Male();
				m1.Id = id;
				m1.Name = name;
				male.Add(m1);
			}
			catch (Exception ex)
			{
				Console.Clear();
				Console.WriteLine("Ошибка при вводе");
				Console.ReadLine();
			}
		}
		public static void AddNewG(List<Female> female)
		{
			try
			{
				Console.Write("Введите ID: ");
				int id = Convert.ToInt32(Console.ReadLine());
				Console.Write("Введите имя: ");
				string name = Console.ReadLine();
				Female f1 = new Female();
				f1.Id = id;
				f1.Name = name;
				female.Add(f1);
			}
			catch (Exception ex)
			{
				Console.Clear();
				Console.WriteLine("Ошибка при вводе");
				Console.ReadLine();
			}
		}
		#endregion
	}
}
