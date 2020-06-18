using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Notify.Backend.Application.Data;
using Notify.Backend.Application.Hubs;
using Notify.Shared.Messaging.Rabbit;
using RabbitMQ.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Notify.Backend.Application.Services;
using Notify.Backend.Application.Models;

namespace Notify.Backend
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddDbContext<NotifyDBContext>(options => options.UseInMemoryDatabase(databaseName: "NotifyDB"));
			services.AddRabbitMQ(Configuration);
			services.AddSignalR().AddJsonProtocol();
			services.AddJwtAuthentication(Configuration);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthentication();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapHub<NotifyHub>("/hub");
			});
		}
	}

	static class StartupExtensions
	{
		public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration Configuration)
		{
			services.Configure<RabbitMQSettings>(Configuration.GetSection("RabbitMQ"));


			services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
			services.AddSingleton<IPooledObjectPolicy<IModel>, RabbitModelPooledObjectPolicy>();
			services.AddSingleton<IRabbitMQManager, RabbitMQManager>();

			return services;
		}

		public static IServiceCollection AddUserDatabase(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<DatabaseSettings>(
					configuration.GetSection(nameof(DatabaseSettings)))

			.AddSingleton<IDatabaseSettings>(sp =>
					sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

			//.AddSingleton<IUserRepository, UserRepository>();

			return services;
		}

		public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
			var jwtSettings = configuration.GetSection("JwtSettings");
			services.Configure<JwtSettings>(jwtSettings)
				.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);

			var secret = configuration.GetSection("JwtSettings:Secret").Value;

			services.AddSingleton<IJwtService, JwtService>();

			services.AddCors();

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			
			.AddJwtBearer(options =>
			{

				options.RequireHttpsMetadata = false;
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
					ValidateIssuer = false,
					ValidateAudience = false,
					//ValidIssuer = configuration["JwtSettings:Issuer"],
					//ValidAudience = configuration["JwtSettings:Audience"],
				};
			});

			

			

			return services;
		}
	}
}
