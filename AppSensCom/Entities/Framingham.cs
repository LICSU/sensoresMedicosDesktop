using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Entities
{
	public class Framingham
	{
		public static int? ScoreAge(int age)
		{
			if (age == 0)
				return null;
			if (age >= 20 && age <= 34)
				return -7;
			else if (age >= 35 && age <= 39)
				return -3;
			else if (age >= 40 && age <= 44)
				return 0;
			else if (age >= 45 && age <= 49)
				return 3;
			else if (age >= 50 && age <= 54)
				return 6;
			else if (age >= 55 && age <= 59)
				return 8;
			else if (age >= 60 && age <= 64)
				return 10;
			else if (age >= 65 && age <= 69)
				return 12;
			else if (age >= 70 && age <= 74)
				return 14;
			else if (age >= 75 && age <= 79)
				return 16;
			else
				return null;
		}

		public static int? ScoreColesterolAge(int col, int age)
		{
			if (age == 0)
				return null;
			if (col < 160 && (age >= 20 && age <= 79))
				return 0;
			else if (col >= 160 && col <= 199 && (age >= 20 && age <= 39))
				return 4;
			else if(col >= 160 && col <= 199 && (age >= 40 && age <= 49))
				return 3;
			else if(col >= 160 && col <= 199 && (age >= 50 && age <= 59))
				return 2;
			else if(col >= 160 && col <= 199 && (age >= 60 && age <= 79))
				return 1;
			else if(col >= 200 && col <= 239 && (age >= 20 && age <= 39))
				return 8;
			else if(col >= 200 && col <= 239 && (age >= 40 && age <= 49))
				return 6;
			else if(col >= 200 && col <= 239 && (age >= 50 && age <= 59))
				return 4;
			else if(col >= 200 && col <= 239 && (age >= 60 && age <= 69))
				return 2;
			else if(col >= 200 && col <= 239 && (age >= 70 && age <= 79))
				return 1;
			else if(col >= 240 && col <= 279 && (age >= 20 && age <= 39))
				return 11;
			else if(col >= 240 && col <= 279 && (age >= 40 && age <= 49))
				return 8;
			else if(col >= 240 && col <= 279 && (age >= 50 && age <= 59))
				return 5;
			else if(col >= 240 && col <= 279 && (age >= 60 && age <= 69))
				return 3;
			else if(col >= 240 && col <= 279 && (age >= 70 && age <= 79))
				return 2;
			else if(col >= 280 && (age >= 20 && age <= 39))
				return 13;
			else if(col >= 280 && (age >= 40 && age <= 49))
				return 10;
			else if(col >= 280 && (age >= 50 && age <= 59))
				return 7;
			else if(col >= 280 && (age >= 60 && age <= 69))
				return 4;
			else if(col >= 280 && (age >= 70 && age <= 79))
				return 2;
			else
				return null;
		}

		public static int? ScoreAgeSmoker(int age)
		{
			if (age == 0)
				return null;
			if (age >= 20 && age <= 39)
				return 9;
			else if (age >= 40 && age <= 49)
				return 7;
			else if (age >= 50 && age <= 59)
				return 4;
			else if (age >= 60 && age <= 69)
				return 2;
			else if (age >= 70 && age <= 79)
				return 1;
			else
				return null;
		}

		public static int? ScoreHdl(int hdl)
		{
			if (hdl < 40)
				return 2;
			else if (hdl >= 40 && hdl <= 49)
				return 1;
			else if (hdl >= 50 && hdl <= 59)
				return 0;
			else if (hdl == 60)
				return -1;
			else
				return null;
		}

		public static int? ScorePresionSistolica(int presionSistolica, bool tratada)
		{
			if (presionSistolica < 120)
				return 0;
			else if (presionSistolica >= 120 && presionSistolica <= 129 && tratada)
				return 1;
			else if (presionSistolica >= 120 && presionSistolica <= 129 && !tratada)
				return 3;
			else if (presionSistolica >= 130 && presionSistolica <= 139 && tratada)
				return 2;
			else if (presionSistolica >= 130 && presionSistolica <= 139 && !tratada)
				return 4;
			else if (presionSistolica >= 140 && presionSistolica <= 149 && tratada)
				return 3;
			else if (presionSistolica >= 140 && presionSistolica <= 149 && !tratada)
				return 5;
			else if (presionSistolica > 160 && tratada)
				return 4;
			else if (presionSistolica > 160 && !tratada)
				return 6;
			else
				return null;
		}
	}
}
