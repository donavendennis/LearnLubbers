namespace Utility
{
	public class FrequencyConstants
	{
		public const string Daily = "Daily";
		public const string Weekly = "Weekly";
		public const string Monthly = "Monthly";
		public const string Yearly = "Yearly";

		public static bool IsValidFrequency(string value)
		{
			return
			  value == Daily ||
			  value == Weekly ||
			  value == Monthly ||
			  value == Yearly;
		}
	}
}
