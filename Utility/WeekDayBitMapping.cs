namespace Utility
{
	public class WeekDayBitMapping
	{
		public byte? days;
		public WeekDayBitMapping(byte? Day) { days = Day; }

		public string WeekDayString()
		{
			string meet = "";
			if (days == null)
			{
				return meet;
			}
			if (((byte)(1 << 0) & days) != 0)
			{
				meet += "M, ";
			}
			if (((byte)(1 << 1) & days) != 0)
			{
				meet += "Tu, ";
			}
			if (((byte)(1 << 2) & days) != 0)
			{
				meet += "W, ";
			}
			if (((byte)(1 << 3) & days) != 0)
			{
				meet += "Th, ";
			}
			if (((byte)(1 << 4) & days) != 0)
			{
				meet += "F, ";
			}

			return meet.Substring(0, meet.Length - 2);
		}

		public static List<int> WeekDayArray(byte? days)
		{
			List<int> daysArray = new List<int>();
			if (days != null)
			{
				if (((byte)(1 << 0) & days) != 0)
				{
					daysArray.Add(1);
				}
				if (((byte)(1 << 1) & days) != 0)
				{
					daysArray.Add(2);
				}
				if (((byte)(1 << 2) & days) != 0)
				{
					daysArray.Add(3);
				}
				if (((byte)(1 << 3) & days) != 0)
				{
					daysArray.Add(4);
				}
				if (((byte)(1 << 4) & days) != 0)
				{
					daysArray.Add(5);
				}
			}
			return daysArray;
		}
		public static byte GetWeekdayBitMap(List<string> daysMet)
		{
			byte bitmap = 0;

			foreach (var day in daysMet)
			{
				switch (day)
				{
					case "Monday":
						bitmap |= 1;
						break;
					case "Tuesday":
						bitmap |= 2;
						break;
					case "Wednesday":
						bitmap |= 4;
						break;
					case "Thursday":
						bitmap |= 8;
						break;
					case "Friday":
						bitmap |= 16;
						break;
				}
			}

			return bitmap;
		}

	}
}
