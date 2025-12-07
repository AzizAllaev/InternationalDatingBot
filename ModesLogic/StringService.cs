using FuzzySharp;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModesLogic
{
	public class StringService
	{
		public static string? NormalizeString(string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return null;

			str = str.ToLower().Trim();
			str = Regex.Replace(str, @"\s+", " ");

			return str;
		}
		public static int? CompareStudents(string input, string reference)
		{
			string? normInput = NormalizeString(input);
			string? normRef = NormalizeString(reference);

			if(normInput == null || normRef == null) 
				return null;

			int score = Fuzz.Ratio(normInput, normRef);

			return score;
		}

		public async Task<bool> CheckApplication(Application newApp, AppDbContext db)
		{
			string newFemale = $"{newApp.FemaleFullName} {newApp.FemaleLyceumName}";
			string newMale = $"{newApp.MaleFullName} {newApp.MaleLyceumName}";

			var allApps = await db.Applications
				.Select(a => new
				{
					a.FemaleFullName,
					a.FemaleLyceumName,
					a.MaleFullName,
					a.MaleLyceumName
				})
				.ToListAsync();

			foreach (var app in allApps)
			{
				string existingFemale = $"{app.FemaleFullName} {app.FemaleLyceumName}";

				int? femaleScore = StringService.CompareStudents(newFemale, existingFemale);
				if (femaleScore >= 85)
				{
					return (true);
				}
			}

			foreach (var app in allApps)
			{
				string existingMale = $"{app.MaleFullName} {app.MaleLyceumName}";

				int? maleScore = StringService.CompareStudents(newMale, existingMale);
				if (maleScore >= 85)
				{
					return (true);
				}
			}

			return (false);
		}

	}
}
