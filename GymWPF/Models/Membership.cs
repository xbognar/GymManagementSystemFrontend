﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymWPF.Models
{
	public class Membership
	{

		public int MembershipID { get; set; }

		public int MemberID { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public string? PaymentType { get; set; }

		public bool IsActive { get; set; }

	}
}
