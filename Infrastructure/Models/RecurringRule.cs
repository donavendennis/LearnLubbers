using System.ComponentModel.DataAnnotations;
using Utility;

namespace Infrastructure.Models
{
	public class RecurringRule
	{
		[Key]
		public int RecurringRuleId { get; set; }
		[Required]
		public int Skip { get; set; }
		[Required]
		public DateTime EndDate { get; set; }


		private string? _frequency;
		[Required]
		public string? Frequency
		{
			get => _frequency;
			set
			{
				if(value == null)
				{
					value = FrequencyConstants.Weekly;
				}
				if (!FrequencyConstants.IsValidFrequency(value))
				{
					throw new ArgumentException("Invalid Frequency Provided");
				}
				_frequency = value;
			}
		}

		public byte? WeekdayBitMap { get; set; }
	}
}
