﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notify.Backend.Application.Commands
{
	public class ConnectToBoardCommand
	{
		public string Board { get; set; }
		public string Id { get; set; }
		public string Key { get; set; }
	}
}