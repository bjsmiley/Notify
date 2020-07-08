using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Notify.Backend.Application.Models;

namespace Notify.Backend.Application.Data
{
	public class NotifyDBContext : DbContext
	{
		public DbSet<Board> Boards { get; set; }

		public NotifyDBContext(DbContextOptions<NotifyDBContext> options)
		: base(options) { }

	}
}
