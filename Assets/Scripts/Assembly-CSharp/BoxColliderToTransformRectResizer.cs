using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(BoxCollider))]
public class BoxColliderToTransformRectResizer : MonoBehaviour
{
	[SerializeField]
	private RectTransform _transform;

	[SerializeField]
	private BoxCollider _collider;

	[SerializeField]
	private float _zSize = 1f;

	[SerializeField]
	private Vector2 _sizeOffsets = Vector2.zero;

	private void Awake()
	{
		SetColliderToBounds();
	}

	private void OnRectTransformDimensionsChange()
	{
		SetColliderToBounds();
	}

	public void SetColliderToBounds()
	{
		if (_collider != null && _transform != null)
		{
			_collider.size = new Vector3(_transform.rect.width + _sizeOffsets.x, _transform.rect.height + _sizeOffsets.y, _zSize);
			_collider.center = new Vector3(_transform.rect.width * (0.5f - _transform.pivot.x), _transform.rect.height * (0.5f - _transform.pivot.y), 0f);
		}
	}

	private void Reset()
	{
		_transform = base.transform as RectTransform;
		_collider = GetComponent<BoxCollider>();
	}
}
