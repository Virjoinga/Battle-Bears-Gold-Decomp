using System;

namespace Analytics.Parameters
{
	public class CharacterParameter : StringParameter
	{
		public enum Type
		{
			OLIVER = 0,
			RIGGS = 1,
			B1000 = 2,
			HUGGABLE = 3,
			TILLMAN = 4,
			GRAHAM = 5,
			WIL = 6,
			BOTCH = 7,
			ASTORIA = 8,
			SABERI = 9,
			SANCHEZ = 10
		}

		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.character;
			}
		}

		public CharacterParameter(Item item)
			: base(FromItem(item).ToString())
		{
		}

		private static Type FromItem(Item item)
		{
			switch (item.name)
			{
			case "Astoria":
				return Type.ASTORIA;
			case "B1000":
				return Type.B1000;
			case "Oliver":
				return Type.OLIVER;
			case "Riggs":
				return Type.RIGGS;
			case "Huggable":
				return Type.HUGGABLE;
			case "Tillman":
				return Type.TILLMAN;
			case "Will":
				return Type.WIL;
			case "Graham":
				return Type.GRAHAM;
			case "Botch":
				return Type.BOTCH;
			case "Saberi":
				return Type.SABERI;
			case "Sanchez":
				return Type.SANCHEZ;
			default:
				throw new Exception("No Character Type defined for " + item.name);
			}
		}
	}
}
