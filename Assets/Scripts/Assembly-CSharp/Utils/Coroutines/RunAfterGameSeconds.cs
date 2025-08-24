using System;
using UnityEngine;

namespace Utils.Coroutines
{
	public class RunAfterGameSeconds : RunAfterSeconds
	{
		protected override float _deltaTime
		{
			get
			{
				return Time.deltaTime;
			}
		}

		public RunAfterGameSeconds(float seconds, Action callback)
			: base(seconds, callback)
		{
		}
	}
}
