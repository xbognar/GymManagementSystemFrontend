﻿namespace GymWPF.Models
{
	public class InactiveMembershipDTO
	{

		public int MembershipID { get; set; }

		public string? MemberName { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public string? Type { get; set; }

	}
}
