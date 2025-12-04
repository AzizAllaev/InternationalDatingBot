using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using Models;


namespace ModesLogic
{
	public class GoogleApiHandler
	{
		public static SheetsService ConnectToSheets(string _jsonkeypath)
		{
			using var stream = File.OpenRead(_jsonkeypath);
			var serviceaccountcredential = ServiceAccountCredential.FromServiceAccountData(stream);

			var credentials = GoogleCredential.FromServiceAccountCredential(serviceaccountcredential).CreateScoped(new[] { SheetsService.Scope.Spreadsheets });

			var service = new SheetsService(new BaseClientService.Initializer() 
			{
				HttpClientInitializer = credentials,
				ApplicationName = "ApplicationLogger"
			});

			return service;
		}
		public static async Task AddRow(SheetsService service, Application app, string sheetId, string table)
		{
			if (app.MaleFullName == null || app.FemaleFullName == null || app.FemaleLyceumName == null || app.MaleLyceumName == null || app.PurposeOfMeeting == null || app.MaleTelegramUserAndPhoneNumber == null)
				return;

			var data = new ValueRange
			{
				Values = new List<IList<object>>
				{
					new List<object> { app.Id, app.TelegramID, app.MaleFullName, app.MaleTelegramUserAndPhoneNumber, app.MaleLyceumName, app.FemaleFullName, app.FemaleLyceumName, app.PurposeOfMeeting }
				}
			};

			var request = service.Spreadsheets.Values.Append(data, sheetId, table);
			request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
			await request.ExecuteAsync();
		}
	}
}
