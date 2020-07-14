using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Notify.Shared.Commands;
using Notify.Shared.Models;

namespace Notify.ClientTest
{
	class Program
	{
		static async Task Main(string[] args)
		{
			await Task.Delay(20000);

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

			var connection = new HubConnectionBuilder()
				.WithUrl("https://localhost:5001/hub", options =>
				{
					options.AccessTokenProvider = () => Task.FromResult(token);
				}).Build();

			await connection.StartAsync();

			connection.On<object>("pushNotifications", (res) => 
			{
				var json = JsonSerializer.Serialize(res, new JsonSerializerOptions { WriteIndented = true });
				Console.WriteLine(json);
			});

			var subcommand = new SubscribeTopicCommand
			{ 
				Topic = "pushNotifications",
				Route = "myapp.push.user.1234"
			};

			await connection.InvokeAsync("subscribe", subcommand);
			var rand = new Random();
			while(true)
			{
				await Task.Delay(rand.Next(1000, 10000));
				if (new Random().Next(1, 10) % 2 == 0)
				{
					var pubcmd = new PublishMessageCommand
					{
						Route = "myapp.push.user.1234",
						Payload = new { Test = "test string", NestObj = subcommand }
					};
					await connection.InvokeAsync("publish", pubcmd);
				}
				else
				{
					var pubcmd = new PublishMessageCommand
					{
						Route = "myapp.push.user.1234",
						Payload = "striiiing."
					};
					await connection.InvokeAsync("publish", pubcmd);
				}
			}





		}
	}
}
