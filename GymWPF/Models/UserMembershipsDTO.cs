namespace GymWPF.Models
{
	public class UserMembershipsDTO
	{

		public int MembershipID { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public string? PaymentType { get; set; }

	}
}
