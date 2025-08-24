using UnityEngine;
using Utils.IEnumerators;

namespace Utils.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class RectTransformAnimatedDrawer : MonoBehaviour
	{
		[SerializeField]
		private float _duration;

		[SerializeField]
		private AnimationCurve _curve;

		[SerializeField]
		private Vector2 _openAnchorMax;

		[SerializeField]
		private Vector2 _openAnchorMin;

		[SerializeField]
		private Vector2 _closedAnchorMax;

		[SerializeField]
		private Vector2 _closedAnchorMin;

		[SerializeField]
		private bool _willClose = true;

		private RectTransform _transform;

		private RectTransformAnimatedMoverIEnumerator _moverRoutine;

		private bool _shouldMove;

		private void Awake()
		{
			_transform = base.transform as RectTransform;
			_moverRoutine = new RectTransformAnimatedMoverIEnumerator(_duration, _curve, _openAnchorMax, _openAnchorMin, _closedAnchorMax, _closedAnchorMin, _transform, true);
		}

		public void Open()
		{
			if (_willClose)
			{
				_moverRoutine.Reverse();
				_willClose = false;
			}
			if (base.gameObject.activeInHierarchy)
			{
				_shouldMove = true;
			}
			else
			{
				_moverRoutine.JumpToTarget();
			}
		}

		public void Close()
		{
			if (!_willClose)
			{
				_moverRoutine.Reverse();
				_willClose = true;
			}
			if (base.gameObject.activeInHierarchy)
			{
				_shouldMove = true;
			}
			else
			{
				_moverRoutine.JumpToTarget();
			}
		}

		private void Update()
		{
			if (_shouldMove && !_moverRoutine.MoveNext())
			{
				_shouldMove = false;
			}
		}

		private void OnDisable()
		{
			_moverRoutine.JumpToTarget();
		}
	}
}
