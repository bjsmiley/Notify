using Notify.Backend.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Notify.Backend.Application.Services
{
	public class JwtService : IJwtService
	{
		private readonly JwtSettings _settings;
		public JwtService(JwtSettings settings)
		{
			_settings = settings;
		}

		public string GenerateToken(string boardName, string userName)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_settings.Secret);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.NameIdentifier, userName),
						new Claim(ClaimTypes.GroupSid, boardName)
					}),
				Expires = DateTime.UtcNow.AddMinutes(_settings.ExpirationInMinutes),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}
	}
}
