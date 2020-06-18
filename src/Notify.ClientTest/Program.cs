using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Notify.Backend.Application.Commands;

namespace Notify.ClientTest
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var client = new HttpClient();
			client.BaseAddress = new Uri("https://localhost:5001");
			Console.WriteLine("Hello World!");

			var body = new CreateBoardCommand
			{
				Board = "myapp",
				Key = "myappkey"
			};

			var json = JsonSerializer.Serialize(body);

			var payload = new StringContent(json, Encoding.UTF8, "application/json");

			var response = await client.PostAsync("create", payload);

			if(!response.IsSuccessStatusCode)
			{
				throw new Exception();
			}

			var body2 = new ConnectToBoardCommand
			{
				Board = "myapp",
				Key = "myappkey",
				Id = "1234"
			};






		}
	}
}
