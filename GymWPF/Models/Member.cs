﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymWPF.Models
{
	public class Member
	{

		public int MemberID { get; set; }

		public string? FirstName { get; set; }

		public string? LastName { get; set; }

		public DateTime? DateOfBirth { get; set; }

		public string? Email { get; set; }

		public string? PhoneNumber { get; set; }

	}
}
