using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notify.Backend.Application.Models
{
	public interface IJwtService
	{
		public string GenerateToken(string boardName, string userName);
	}
}
