﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class Like
	{
		public int Id { get; set; }

		public UserProfile? Male { get; set; }
		public UserProfile? Female { get; set; }
	}
}
