using System;
using UnityEngine;

namespace Utils.Coroutines
{
	public class RunAfterRealtimeSeconds : RunAfterSeconds
	{
		protected override float _deltaTime
		{
			get
			{
				return Time.unscaledDeltaTime;
			}
		}

		public RunAfterRealtimeSeconds(float seconds, Action callback)
			: base(seconds, callback)
		{
		}
	}
}
