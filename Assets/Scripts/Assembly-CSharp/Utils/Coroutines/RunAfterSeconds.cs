using System;
using System.Collections;

namespace Utils.Coroutines
{
	public abstract class RunAfterSeconds : IEnumerator
	{
		protected float _secondsToWait;

		protected Action _callback;

		protected float _timer;

		protected abstract float _deltaTime { get; }

		public object Current
		{
			get
			{
				return _timer / _secondsToWait;
			}
		}

		public RunAfterSeconds(float seconds, Action callback)
		{
			_secondsToWait = seconds;
			_callback = callback;
			_timer = 0f;
		}

		public bool MoveNext()
		{
			if (_timer < _secondsToWait)
			{
				_timer += _deltaTime;
				return true;
			}
			if (_callback != null)
			{
				_callback();
			}
			return false;
		}

		public void Reset()
		{
			_timer = 0f;
		}
	}
}
