using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymWPF.Models
{
	public class Chip
	{

		public int ChipID { get; set; }

		public int MemberID { get; set; }

		public string? ChipInfo { get; set; }

		public bool IsActive { get; set; }

	}
}
