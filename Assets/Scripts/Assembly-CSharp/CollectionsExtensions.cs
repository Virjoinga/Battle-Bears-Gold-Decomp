using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CollectionsExtensions
{
	private static System.Random rng = new System.Random();

	public static T Random<T>(this IEnumerable<T> e)
	{
		return e.ElementAt(UnityEngine.Random.Range(0, e.Count()));
	}

	public static T Random<T>(this IList<T> l)
	{
		return l[UnityEngine.Random.Range(0, l.Count)];
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = rng.Next(num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}
}
