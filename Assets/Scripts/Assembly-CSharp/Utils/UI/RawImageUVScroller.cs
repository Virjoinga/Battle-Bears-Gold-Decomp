using UnityEngine;
using UnityEngine.UI;

namespace Utils.UI
{
	[RequireComponent(typeof(RawImage))]
	public class RawImageUVScroller : MonoBehaviour
	{
		[SerializeField]
		private RawImage _image;

		[SerializeField]
		private float _x;

		[SerializeField]
		private float _y;

		private void Update()
		{
			Rect uvRect = _image.uvRect;
			uvRect.x += _x * Time.deltaTime;
			uvRect.y += _y * Time.deltaTime;
			_image.uvRect = uvRect;
		}
	}
}
