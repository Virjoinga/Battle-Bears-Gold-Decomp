using System;
using System.Collections;
using UnityEngine;

namespace Utils.IEnumerators
{
	public class RectTransformAnimatedMoverIEnumerator : IEnumerator
	{
		private float _duration;

		private AnimationCurve _curve;

		private Vector2 _startAnchorMax;

		private Vector2 _startAnchorMin;

		private Vector2 _endAnchorMax;

		private Vector2 _endAnchorMin;

		private RectTransform _transform;

		private bool _useGameTime;

		private float _timer;

		private Action _moveAction;

		private bool _movingTowardsEnd;

		public object Current
		{
			get
			{
				return _timer / _duration;
			}
		}

		public RectTransformAnimatedMoverIEnumerator(float duration, AnimationCurve curve, Vector2 startAnchorMax, Vector2 startAnchorMin, Vector2 endAnchorMax, Vector2 endAnchorMin, RectTransform transform, bool useGameTime)
		{
			_duration = duration;
			_curve = curve;
			_startAnchorMax = startAnchorMax;
			_startAnchorMin = startAnchorMin;
			_endAnchorMax = endAnchorMax;
			_endAnchorMin = endAnchorMin;
			_transform = transform;
			_useGameTime = useGameTime;
			_moveAction = MoveTowardsEnd;
			_movingTowardsEnd = true;
		}

		public void Reverse()
		{
			if (_movingTowardsEnd)
			{
				_moveAction = MoveTowardsStart;
				_movingTowardsEnd = false;
			}
			else
			{
				_moveAction = MoveTowardsEnd;
				_movingTowardsEnd = true;
			}
		}

		public void JumpToTarget()
		{
			_timer = ((!_movingTowardsEnd) ? 0f : _duration);
			LerpTransform(T());
		}

		public bool MoveNext()
		{
			if (NotFinishedMoving())
			{
				_moveAction();
				return true;
			}
			JumpToTarget();
			return false;
		}

		private bool NotFinishedMoving()
		{
			if (_movingTowardsEnd)
			{
				return _timer < _duration;
			}
			return _timer > 0f;
		}

		private void MoveTowardsEnd()
		{
			_timer += ((!_useGameTime) ? Time.unscaledDeltaTime : Time.deltaTime);
			LerpTransform(T());
		}

		private void MoveTowardsStart()
		{
			_timer -= ((!_useGameTime) ? Time.unscaledDeltaTime : Time.deltaTime);
			LerpTransform(T());
		}

		private void LerpTransform(float t)
		{
			_transform.anchorMax = Vector2.Lerp(_startAnchorMax, _endAnchorMax, t);
			_transform.anchorMin = Vector2.Lerp(_startAnchorMin, _endAnchorMin, t);
			RectTransform transform = _transform;
			Vector2 zero = Vector2.zero;
			_transform.offsetMin = zero;
			transform.offsetMax = zero;
		}

		private float T()
		{
			return _curve.Evaluate(_timer / _duration);
		}

		public void Reset()
		{
			_timer = 0f;
			_moveAction = MoveTowardsEnd;
			_movingTowardsEnd = true;
		}
	}
}
