using System;
using System.Collections.Generic;
using SkyVu.Common.Enums;

namespace SkyVuEngine.Core.Ranking
{
	internal class CategoryStrategy : RankingStrategy
	{
		public enum CategoryRank
		{
			None = 0,
			Beginner = 1,
			Bronze = 2,
			Silver = 3,
			Gold = 4,
			Platnium = 5,
			Master = 6
		}

		private Dictionary<int, float> _difficultyModifier = new Dictionary<int, float>();

		private Dictionary<int, float> _rewardModifier = new Dictionary<int, float>();

		private Dictionary<int, float> _rankPercentile = new Dictionary<int, float>();

		public CategoryRank Rank { get; set; }

		public CategoryStrategy(Games game)
			: base(game)
		{
		}

		public float GetDifficultyModifier()
		{
			return _difficultyModifier[(int)Rank];
		}

		public float GetRewardModifier()
		{
			return _rewardModifier[(int)Rank];
		}

		public override bool CalculateRankValue()
		{
			base.TotalPoints += 20;
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			int[] array = new int[1000];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = i * 20;
			}
			string[] names = Enum.GetNames(typeof(CategoryRank));
			int num = array.Length;
			for (int j = 0; j < names.Length; j++)
			{
				if ((int)(_rankPercentile[j] * (float)num) >= num)
				{
					dictionary.Add(j, array[num - 1]);
				}
				else
				{
					dictionary.Add(j, array[(int)(_rankPercentile[j] * (float)num)]);
				}
				if (names.Length == j + 1)
				{
					Rank = (CategoryRank)j;
					break;
				}
				if (base.TotalPoints <= dictionary[j])
				{
					Rank = (CategoryRank)j;
					break;
				}
			}
			return true;
		}
	}
}
