using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Notify.Backend.Application.Commands;
using Notify.Backend.Application.Models;

namespace Notify.ClientTest
{
	class Program
	{
		static async Task Main(string[] args)
		{
			await Task.Delay(10000);

			var client = new HttpClient();
			client.BaseAddress = new Uri("https://localhost:5001/api/board/");
			Console.WriteLine("Hello World!");

			var body = new CreateBoardCommand
			{
				Board = "myapp",
				Key = "myappkey"
			};

			var json = JsonSerializer.Serialize(body);

			var payload = new StringContent(json, Encoding.UTF8, "application/json");

			var response = await client.PostAsync("create", payload);
			response.EnsureSuccessStatusCode();

			var body2 = new ConnectToBoardCommand
			{
				Board = "myapp",
				Key = "myappkey",
				Name = "1234"
			};

			var json2 = JsonSerializer.Serialize(body2);

			var reqMessage = new HttpRequestMessage(HttpMethod.Get, client.BaseAddress + "connect");
			

			reqMessage.Content = new StringContent(json2, Encoding.UTF8, "application/json");

			var response2 = await client.SendAsync(reqMessage);
			response2.EnsureSuccessStatusCode();

			var str = await response2.Content.ReadAsStringAsync();
			var token = JsonSerializer.Deserialize<TokenModel>(str, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }).Token;


			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);








		}
	}
}
