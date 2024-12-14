namespace GymWPF.Models
{
	public class ChipUpdateRequest
	{
		public int ChipID { get; set; }
		
		public int NewMemberID { get; set; }
		
		public bool? IsActive { get; set; }
	}

}
