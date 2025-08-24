using UnityEngine;

namespace Analytics.Parameters
{
	public class StageParameter : StringParameter
	{
		public enum Stage
		{
			ABUSEMENT_PARK = 0,
			AZTEC_ATTACK = 1,
			DESERT_AIRMINE = 2,
			FROZEN_PUNDRA = 3,
			GOLD_DIGGER = 4,
			HAUNTED_CASTLE = 5,
			HIP_TEST_LAB = 6,
			MARECRAFT = 7,
			NO_BEARS_LAND = 8,
			SHORTEST_PARSEC = 9,
			SKATE_OR_DIE = 10,
			SKY_VIEW = 11,
			SPACE_ODDITY = 12,
			TOXIC_TERROR = 13,
			RAXUS_PRIME = 14,
			CASTLE_ROYALE = 15,
			unknown = 16
		}

		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.stage;
			}
		}

		public StageParameter(string stage)
			: base(StageFromString(stage).ToString())
		{
		}

		private static Stage StageFromString(string stage)
		{
			switch (stage)
			{
			case "Factory":
				return Stage.HIP_TEST_LAB;
			case "BuddhaLevel":
				return Stage.SPACE_ODDITY;
			case "Skyscraper":
				return Stage.SKY_VIEW;
			case "Facingtemples":
			case "FacingTemples":
				return Stage.AZTEC_ATTACK;
			case "AbusementPark":
				return Stage.ABUSEMENT_PARK;
			case "Mine":
				return Stage.GOLD_DIGGER;
			case "ShortestParsec":
				return Stage.SHORTEST_PARSEC;
			case "Marecraft":
				return Stage.MARECRAFT;
			case "SkatePark":
				return Stage.SKATE_OR_DIE;
			case "X":
				return Stage.TOXIC_TERROR;
			case "Castle2":
				return Stage.HAUNTED_CASTLE;
			case "CastleRoyale":
				return Stage.CASTLE_ROYALE;
			case "Desert":
				return Stage.DESERT_AIRMINE;
			case "IceIceBaby":
				return Stage.FROZEN_PUNDRA;
			case "RaxusPrime":
				return Stage.RAXUS_PRIME;
			case "FieldTest":
				return Stage.NO_BEARS_LAND;
			default:
				Debug.LogError("No Stage defined for string stage " + stage);
				return Stage.unknown;
			}
		}
	}
}
